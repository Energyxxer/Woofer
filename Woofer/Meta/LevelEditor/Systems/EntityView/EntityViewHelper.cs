using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EntityComponentSystem.Components;
using EntityComponentSystem.Entities;
using EntityComponentSystem.Scenes;
using EntityComponentSystem.Util;
using WooferGame.Common;
using WooferGame.Meta.LevelEditor.Systems.ComponentView;
using WooferGame.Meta.LevelEditor.Systems.CursorModes;
using WooferGame.Meta.LevelEditor.Systems.InputModes;
using WooferGame.Systems;
using WooferGame.Systems.Physics;
using WooferGame.Systems.Generators;
using WooferGame.Systems.Visual;
using WooferGame.Systems.Visual.Animation;
using WooferGame.Meta.LevelEditor.Systems.AnimationView;
using WooferGame.Systems.Sounds;
using WooferGame.Meta.LevelEditor.Systems.SoundView;
using WooferGame.Meta.LevelEditor.Systems.ObjectEditor;

namespace WooferGame.Meta.LevelEditor.Systems.EntityView
{
    class EntityViewHelper
    {
        internal readonly EntityViewSystem Owner;
        internal long Id;

        internal Dictionary<string, ComponentSummary> Components = new Dictionary<string, ComponentSummary>();
        
        public EntityViewHelper(EntityViewSystem owner) => Owner = owner;

        public void Update(long id)
        {
            Id = id;
            Components.Clear();
            Entity entity = Owner.Owner.Entities[Id];
            if (entity == null) return;
            foreach(Component component in entity.Components)
            {
                Components[component.ComponentName] = new ComponentSummary(this, component);
            }
        }
    }

    internal class ComponentSummary
    {
        public readonly EntityViewHelper Owner;
        public string ComponentName;
        public Dictionary<string, IMemberSummary> Members = new Dictionary<string, IMemberSummary>();

        public ComponentSummary(EntityViewHelper owner, Component component)
        {
            this.Owner = owner;
            ComponentName = component.ComponentName;
            
            foreach(PropertyInfo property in component.GetType().GetProperties())
            {
                if (property.DeclaringType == typeof(Component)) continue;
                if (property.GetCustomAttribute(typeof(HideInInspector)) != null) continue;
                Members[property.Name] = new PropertySummary(this, property, component);
            }

            foreach (FieldInfo field in component.GetType().GetFields())
            {
                if (field.DeclaringType == typeof(Component)) continue;
                if (field.GetCustomAttribute(typeof(HideInInspector)) != null) continue;
                Members[field.Name] = new FieldSummary(this, field, component);
            }
        }
    }

    internal class ObjectSummary
    {
        public readonly Scene Scene;
        public Dictionary<string, IMemberSummary> Members = new Dictionary<string, IMemberSummary>();

        public ObjectSummary(Scene scene, object obj)
        {
            Scene = scene;
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (property.DeclaringType == typeof(Component)) continue;
                if (property.GetCustomAttribute(typeof(HideInInspector)) != null) continue;
                Members[property.Name] = new PropertySummary(scene, property, obj);
            }

            foreach (FieldInfo field in obj.GetType().GetFields())
            {
                if (field.DeclaringType == typeof(Component)) continue;
                if (field.GetCustomAttribute(typeof(HideInInspector)) != null) continue;
                Members[field.Name] = new FieldSummary(scene, field, obj);
            }
        }
    }

    internal interface IMemberSummary
    {
        Scene Scene { get; }
        ComponentSummary Owner { get; }
        string Name { get; }
        TextUnit Label { get; }
        bool CanSet { get; }

        InspectorEditType EditType { get; }

        Type DataType { get; }

        object GetValue();
        bool SetValue(object obj);
    }

    internal class PropertySummary : IMemberSummary
    {
        public ComponentSummary Owner { get; private set; }
        public Scene Scene { get; private set; }
        private readonly PropertyInfo Property;
        private readonly object Object;

        public string Name { get; private set; }
        public bool CanSet { get; private set; }

        public InspectorEditType EditType { get; private set; }

        public Type DataType => Property.PropertyType;

        public static readonly Sprite CheckedSprite = new Sprite("editor", new Rectangle(0, 0, 8, 8), new Rectangle(8, 48, 8, 8));
        public static readonly Sprite UncheckedSprite = new Sprite("editor", new Rectangle(0, 0, 8, 8), new Rectangle(0, 48, 8, 8));

        public TextUnit Label => (Property.PropertyType == typeof(bool)) ? new TextUnit((bool)GetValue() ? CheckedSprite : UncheckedSprite, Name) :
            new TextUnit(Name + ": " + GetValue());

        public PropertySummary(Scene scene, PropertyInfo property, object obj) : this(null, scene, property, obj) { }

        public PropertySummary(ComponentSummary owner, PropertyInfo property, object obj) : this(owner, owner.Owner.Owner.Owner, property, obj) { }

        public PropertySummary(ComponentSummary owner, Scene scene, PropertyInfo property, object obj)
        {
            this.Owner = owner;
            this.Scene = scene;
            this.Property = property;
            this.Object = obj;

            Name = property.Name;
            CanSet = property.CanWrite;

            if (property.GetCustomAttribute(typeof(InspectorAttribute)) is InspectorAttribute attribute)
            {
                EditType = attribute.Flags;
            }
            else EditType = InspectorEditType.Default;
        }

        public object GetValue() => Property.GetValue(Object);

        public bool SetValue(object val)
        {
            if(CanSet && Property.PropertyType.IsAssignableFrom(val.GetType()))
            {
                Property.SetValue(Object, val);
                return true;
            }
            return false;
        }
    }

    internal class FieldSummary : IMemberSummary
    {
        public ComponentSummary Owner { get; private set; }
        public Scene Scene { get; private set; }
        private readonly FieldInfo Field;
        private readonly object Object;

        public string Name { get; private set; }
        public bool CanSet { get; private set; }

        public InspectorEditType EditType { get; private set; }

        public Type DataType => Field.FieldType;

        public TextUnit Label => (Field.FieldType == typeof(bool)) ? new TextUnit((bool)GetValue() ? PropertySummary.CheckedSprite : PropertySummary.UncheckedSprite, Name) :
            new TextUnit(Name + ": " + GetValue());

        public FieldSummary(Scene scene, FieldInfo field, object obj) : this(null, scene, field, obj) { }

        public FieldSummary(ComponentSummary owner, FieldInfo field, object obj) : this(owner, owner.Owner.Owner.Owner, field, obj) { }

        public FieldSummary(ComponentSummary owner, Scene scene, FieldInfo field, object obj)
        {
            this.Owner = owner;
            this.Scene = scene;
            this.Field = field;
            this.Object = obj;

            Name = field.Name;
            CanSet = !field.IsInitOnly;
            
            if (field.GetCustomAttribute(typeof(InspectorAttribute)) is InspectorAttribute attribute)
            {
                EditType = attribute.Flags;
            }
            else EditType = InspectorEditType.Default;
        }

        public object GetValue() => Field.GetValue(Object);

        public bool SetValue(object val)
        {
            if (CanSet && Field.FieldType.IsAssignableFrom(val.GetType()))
            {
                Field.SetValue(Object, val);
                return true;
            }
            return false;
        }
    }

    internal static class MemberEdits
    {
        public static bool TriggerEdit(this IMemberSummary member, bool forceDirect = false)
        {
            Type type = member.DataType;
            if (type == typeof(bool))
            {
                member.SetValue(!(bool)member.GetValue());
                return false;
            }
            else if (type == typeof(Vector2D))
            {
                if (!forceDirect && member.EditType == InspectorEditType.Position)
                {
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent((Vector2D)member.GetValue()));
                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("move_cursor_mode", null));
                    member.Scene.Events.InvokeEvent(new StartMoveModeEvent((vec, def) => member.SetValue(vec)));
                    return true;
                }
                else if (!forceDirect && member.EditType == InspectorEditType.Offset)
                {
                    Vector2D pivot = Vector2D.Empty;
                    if (member.Owner != null)
                    {
                        Entity entity = member.Scene.Entities[member.Owner.Owner.Id];
                        pivot = entity.Components.Get<Spatial>().Position;
                    }
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent(((Vector2D)member.GetValue()) + pivot));
                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("move_cursor_mode", null));
                    member.Scene.Events.InvokeEvent(new StartMoveModeEvent((vec, def) => member.SetValue(vec - pivot)));
                    return true;
                }
                else
                {
                    Vector2D vec = (Vector2D)member.GetValue();

                    StartNumberInputEvent.OnSubmit onReceiveY = v =>
                    {
                        vec.Y = v;
                        member.SetValue(vec);
                    };

                    StartNumberInputEvent.OnSubmit onReceiveX = (v =>
                    {
                        vec.X = v;
                        member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                        member.Scene.Events.InvokeEvent(new StartNumberInputEvent(vec.Y, onReceiveY, null) { Label = "Vector2D.Y" });
                    });

                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                    member.Scene.Events.InvokeEvent(new StartNumberInputEvent(vec.X, onReceiveX, null) { Label = "Vector2D.X" });
                    return true;
                }
            }
            else if (type == typeof(Size))
            {
                Size vec = (Size)member.GetValue();

                StartNumberInputEvent.OnSubmit onReceiveHeight = v =>
                {
                    vec.Height = v;
                    member.SetValue(vec);
                };

                StartNumberInputEvent.OnSubmit onReceiveWidth = (v =>
                {
                    vec.Width = v;
                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                    member.Scene.Events.InvokeEvent(new StartNumberInputEvent(vec.Height, onReceiveHeight, true, null) { Label = "Size.Height" });
                });

                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                member.Scene.Events.InvokeEvent(new StartNumberInputEvent(vec.Width, onReceiveWidth, true, null) { Label = "Size.Width" });
                return true;
            }
            else if (type == typeof(System.Drawing.Size))
            {
                System.Drawing.Size vec = (System.Drawing.Size)member.GetValue();

                StartNumberInputEvent.OnSubmit onReceiveHeight = v =>
                {
                    vec.Height = (int)v;
                    member.SetValue(vec);
                };

                StartNumberInputEvent.OnSubmit onReceiveWidth = (v =>
                {
                    vec.Width = (int)v;
                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                    member.Scene.Events.InvokeEvent(new StartNumberInputEvent(vec.Height, onReceiveHeight, false, null) { Label = "Size.Height" });
                });

                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                member.Scene.Events.InvokeEvent(new StartNumberInputEvent(vec.Width, onReceiveWidth, false, null) { Label = "Size.Width" });
                return true;
            }
            else if (type == typeof(double))
            {
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                member.Scene.Events.InvokeEvent(new StartNumberInputEvent((double)member.GetValue(), v => member.SetValue(v), null));
                return true;
            }
            else if (type == typeof(float))
            {
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                member.Scene.Events.InvokeEvent(new StartNumberInputEvent((float)member.GetValue(), v => member.SetValue((float)v), null));
                return true;
            }
            else if (type == typeof(int))
            {
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                member.Scene.Events.InvokeEvent(new StartNumberInputEvent((int)member.GetValue(), v => member.SetValue((int)v), false, null));
                return true;
            }
            else if (type == typeof(string))
            {
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("text_input", null));
                member.Scene.Events.InvokeEvent(new StartTextInputEvent((string)member.GetValue() ?? "", v => member.SetValue(v), null));
                return true;
            }
            else if (type == typeof(CollisionBox))
            {
                Vector2D pivot = Vector2D.Empty;
                if (member.Owner != null)
                {
                    Entity entity = member.Scene.Entities[member.Owner.Owner.Id];
                    pivot = entity.Components.Get<Spatial>().Position;
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent(pivot));
                }
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("collision_cursor_mode", null));
                member.Scene.Events.InvokeEvent(new StartCollisionModeEvent(pivot, new[] { (CollisionBox)member.GetValue() }, v => member.SetValue(v[0]), false));
                return true;
            }
            else if (type == typeof(CollisionBox[]))
            {
                Vector2D pivot = Vector2D.Empty;
                if (member.Owner != null)
                {
                    Entity entity = member.Scene.Entities[member.Owner.Owner.Id];
                    pivot = entity.Components.Get<Spatial>().Position;
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent(pivot));
                }
                member.Owner.Owner.Owner.Owner.Events.InvokeEvent(new ForceModalChangeEvent("collision_cursor_mode", null));
                member.Owner.Owner.Owner.Owner.Events.InvokeEvent(new StartCollisionModeEvent(pivot, (CollisionBox[])member.GetValue(), v => member.SetValue(v.ToArray()), true));
                return true;
            }
            else if (type == typeof(List<long>))
            {
                Vector2D pivot = Vector2D.Empty;
                long forbidden = 0;
                if (member.Owner != null)
                {
                    Entity entity = member.Scene.Entities[member.Owner.Owner.Id];
                    forbidden = entity.Id;
                    pivot = entity.Components.Get<Spatial>().Position;
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent(pivot));
                }
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("entity_selection_cursor_mode", null));
                member.Scene.Events.InvokeEvent(new StartEntitySelectionModeEvent(pivot, (List<long>)member.GetValue(), v => member.SetValue(v), true) { ForbiddenLink = forbidden });
                return true;
            }
            else if (type == typeof(long))
            {
                Vector2D pivot = Vector2D.Empty;
                long forbidden = 0;
                if (member.Owner != null)
                {
                    Entity entity = member.Scene.Entities[member.Owner.Owner.Id];
                    forbidden = entity.Id;
                    pivot = entity.Components.Get<Spatial>()?.Position ?? Vector2D.Empty;
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent(pivot));
                }
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("entity_selection_cursor_mode", null));
                member.Scene.Events.InvokeEvent(new StartEntitySelectionModeEvent(pivot, new List<long> { (long)member.GetValue() }, v => member.SetValue(v.First()), false) { ForbiddenLink = forbidden });
                return true;
            }
            else if (type == typeof(List<Sprite>))
            {
                Vector2D origin = Vector2D.Empty;
                if (member.Owner != null)
                {
                    Entity entity = member.Scene.Entities[member.Owner.Owner.Id];
                    origin += entity.Components.Get<Spatial>()?.Position ?? Vector2D.Empty;
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent(origin));
                }
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("sprite_cursor_mode", null));
                member.Scene.Events.InvokeEvent(new StartSpriteModeEvent(origin, (List<Sprite>)member.GetValue(), v => member.SetValue(v), true));
                return true;
            } else if(type == typeof(Sprite))
            {
                member.Scene.Events.InvokeEvent(new StartSpriteSourceEditEvent((Sprite)member.GetValue()));
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("sprite_source_view", null));
                return true;
            }
            else if (type == typeof(Rectangle))
            {
                Rectangle rect = (Rectangle)member.GetValue();

                StartNumberInputEvent.OnSubmit onReceiveHeight = v =>
                {
                    rect.Height = v;
                    member.SetValue(rect);
                };

                StartNumberInputEvent.OnSubmit onReceiveWidth = v =>
                {
                    rect.Width = v;
                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                    member.Scene.Events.InvokeEvent(new StartNumberInputEvent(rect.Height, onReceiveHeight, null) { Label = "Height=" });
                };

                StartNumberInputEvent.OnSubmit onReceiveY = v =>
                {
                    rect.Y = v;
                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                    member.Scene.Events.InvokeEvent(new StartNumberInputEvent(rect.Width, onReceiveWidth, null) { Label = "Width=" });
                };

                StartNumberInputEvent.OnSubmit onReceiveX = (v =>
                {
                    rect.X = v;
                    member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                    member.Scene.Events.InvokeEvent(new StartNumberInputEvent(rect.Y, onReceiveY, null) { Label = "Y=" });
                });

                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("number_input", null));
                member.Scene.Events.InvokeEvent(new StartNumberInputEvent(rect.X, onReceiveX, null) { Label = "X=" });
                return true;
            }
            else if (type == typeof(bool[,]))
            {
                Vector2D origin = Vector2D.Empty;
                if (member.Owner != null)
                {
                    Entity entity = member.Scene.Entities[member.Owner.Owner.Id];
                    origin += entity.Components.Get<Spatial>().Position;
                    origin += entity.Components.Get<RoomBuilder>().Offset;
                    member.Scene.Events.InvokeEvent(new ForceMoveCursorEvent(origin));
                }
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("room_builder_mode", null));
                member.Scene.Events.InvokeEvent(new StartRoomBuilderModeEvent(origin, (bool[,])member.GetValue(), v => member.SetValue(v)));
                return true;
            }
            else if (type == typeof(List<AnimatedSprite>))
            {
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("animation_view", null));
                member.Scene.Events.InvokeEvent(new SelectAnimationEvent((List<AnimatedSprite>)member.GetValue()));
                return true;
            }
            else if (type == typeof(TextUnit))
            {
                member.Scene.Events.InvokeEvent(new StartObjectEditEvent((TextUnit)member.GetValue(), "Edit Text", null));
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("object_editor", null));
                return true;
            }
            else if (type == typeof(List<Sound>))
            {
                member.Scene.Events.InvokeEvent(new SelectSoundEvent((List<Sound>)member.GetValue()));
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("sound_view", null));
                return true;
            }
            else if (type.IsEnum)
            {
                member.Scene.Events.InvokeEvent(new StartEnumSelectEvent(member.Name, type.GetEnumNames().ToList(), v => member.SetValue(Enum.Parse(type, v)), null));
                member.Scene.Events.InvokeEvent(new ForceModalChangeEvent("enum_select", null));
                return true;
            }
            return false;
        }
    }
}
