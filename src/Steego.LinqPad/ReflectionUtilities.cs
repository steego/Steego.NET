using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Steego.LinqPad {
    public class ReflectionUtilities {
        public interface ITypeInfo {
            string Name { get; }
            bool IsPrimitive { get; }
            bool IsEnumerable { get; }
            bool IsTypedEnumerable { get; }
            IEnumerable<IMemberInfo> Members { get; }
            IMemberInfo this[string name] { get; }
        }

        public interface IMemberInfo {
            string Name { get; }
            ITypeInfo TypeInfo { get; }
            Type Type { get; }
            Func<object, object> GetValue { get; }
            MemberInfo ReflectionMemberInfo { get; }
        }

        public class TypeInfo<T> : ITypeInfo {

            static TypeInfo() {
                var ObjectType = typeof(T);
                var IsString = ObjectType == typeof(string);
                var Interfaces = ObjectType.GetInterfaces();

                _Name = ObjectType.Name;
                _IsPrimitive = (ObjectType.IsValueType | IsString);
                _IsEnumerable = ObjectType.IsArray || typeof(IEnumerable).IsAssignableFrom(ObjectType);
                _IsTypedEnumerable = _IsEnumerable && (
                    from i in Interfaces
                    where i.IsGenericType()
                    where i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select i).Any();
                _Members = (
                    from m in GetMemberGetters<T>()
                    select new MemberInfo(m.Name, m.Type, m.Get)).ToArray();

            }

            public class MemberInfo : IMemberInfo {
                public MemberInfo(string name, Type type, Func<object, object> getter) {
                    Name = name;
                    GetValue = getter;
                    Type = type;
                    TypeInfo = (ITypeInfo) Activator.CreateInstance(typeof(TypeInfo<>).MakeGenericType(type));
                }

                public System.Func<object, object> GetValue { get; }
                public System.Reflection.MemberInfo ReflectionMemberInfo { get; }

                public string Name { get; }

                public ITypeInfo TypeInfo { get; }

                public System.Type Type { get; }

                //public System.Reflection.MemberInfo MemberInfo {get;}
            }

            private static readonly bool _IsEnumerable;
            public bool IsEnumerable => _IsEnumerable;

            private static readonly bool _IsPrimitive;
            public bool IsPrimitive => _IsPrimitive;

            private static readonly bool _IsTypedEnumerable;
            public bool IsTypedEnumerable => _IsTypedEnumerable;

            private static string _Name;
            public string Name {
                get {
                    return _Name;
                }
            }

            private static IMemberInfo[] _Members;
            public System.Collections.Generic.IEnumerable<IMemberInfo> Members {
                get {
                    return _Members;
                }
            }

            public IMemberInfo this[string name] {
                get {
                    var result = (
                        from m in Members
                        where m.Name == name
                        select m).FirstOrDefault();
                    if(result == null) {
                        result = new MemberInfo(name, typeof(object), (o) => null);
                    }
                    return result;
                }
            }
        }

        public struct MemberGetter {
            public string Name;
            public Type Type;
            public Func<object, object> Get;

            public bool IsEnumerable {
                get {
                    if(Type.IsArray) {
                        return true;
                    }
                    if(Type.GetInterface("IEnumerable") != null) {
                        return true;
                    }
                    if(Type.HasElementType) {
                        return true;
                    }
                    return false;
                }
            }
        }

        public MemberGetter GetMemberGetter<T>(string name) {
            var Found = (
                from m in GetMemberGetters<T>()
                where m.Name == name
                select m).ToArray();

            if(Found.Any()) {
                return Found.FirstOrDefault();
            } else {
                var Getter = new MemberGetter();
                Getter.Name = name;
                Getter.Type = typeof(object);
                Getter.Get = (o) => null;
                return Getter;
            }

        }

        public MemberGetter[] GetMemberGetters(Type ObjectType) {
            try {
                var Flags = BindingFlags.Public | BindingFlags.Instance;
                var Properties = (
                    from p in ObjectType.GetProperties((System.Reflection.BindingFlags) Flags)
                    where p.IsSpecialName == false
                    where p.GetIndexParameters().Length == 0 && p.CanRead && p.IsSpecialName == false
                    let param = E.Parameter(typeof(object), "x")
                    let cparam = E.Convert(param, ObjectType)
                    let IsType = E.TypeIs(param, ObjectType)
                    let GetProp = E.Convert(E.Property(cparam, p), typeof(object))
                    let Check = E.Lambda<Func<object, object>>(E.Condition(IsType, GetProp, E.Constant(null)), param).Compile()
                    select new MemberGetter {
                        Name = p.Name,
                        Type = p.PropertyType,
                        Get = Check
                    }).ToArray();

                var Fields = (
                    from f in ObjectType.GetFields((System.Reflection.BindingFlags) Flags)
                    where f.IsPublic && f.IsSpecialName == false
                    let param = E.Parameter(typeof(object), "x")
                    let cparam = E.Convert(param, ObjectType)
                    let IsType = E.TypeIs(param, ObjectType)
                    let GetField = E.Convert(E.Field(cparam, f), typeof(object))
                    let Check = E.Lambda<Func<object, object>>(E.Condition(IsType, GetField, E.Constant(null)), param).Compile()
                    select new MemberGetter {
                        Name = f.Name,
                        Type = f.FieldType,
                        Get = Check
                    }).ToArray();

                return Fields.Union(Properties).ToArray();

            } catch(Exception ex) {
                throw new Exception("Error getting getters for type: {0}".Fill(ObjectType.Name), ex);
            }
        }

        public MemberGetter[] GetMemberGetters<T>() {
            var ObjectType = typeof(T);
            try {
                var Flags = BindingFlags.Public | BindingFlags.Instance;
                //INSTANT C# TODO TASK: Lambda expressions cannot be assigned to 'var':
                var Properties =
                    from p in ObjectType.GetProperties(Flags)
                    where p.IsSpecialName == false
                    where p.GetIndexParameters().Length == 0 && p.CanRead && p.IsSpecialName == false
                    let param = E.Parameter(ObjectType, "x")
                    let GetProp = E.Lambda<Func<T, object>>(E.Convert(E.Property(param, p), typeof(object)), param).Compile()
                    select new MemberGetter {
                        Name = p.Name,
                        Type = p.PropertyType,
                        Get = (o) => ((o is T) ? GetProp(o) : null)
                    };

                //INSTANT C# TODO TASK: Lambda expressions cannot be assigned to 'var':
                var Fields =
                    from f in ObjectType.GetFields(Flags)
                    where f.IsPublic && f.IsSpecialName == false
                    let param = E.Parameter(ObjectType, "x")
                    let GetField = E.Lambda<Func<T, object>>(E.Convert(E.Field(param, f), typeof(object)), param).Compile()
                    select new MemberGetter {
                        Name = f.Name,
                        Type = f.FieldType,
                        Get = (o) => ((o is T) ? GetField((T) o) : null)
                    };

                return Fields.Union(Properties).ToArray();

            } catch(Exception ex) {
                throw new Exception("Error getting getters for type: {0}".Fill(ObjectType.Name), ex);
            }
        }

        public object GetPropertyValue(object o, PropertyInfo p) {
            try {
                return p.GetValue(o, null);
            } catch(Exception ex) {
                return ex;
            }
        }

        public interface ITypeReader {
            IFieldReader[] Fields { get; }
        }

        public interface IFieldReader {
            string Name { get; }
            string Title { get; }
            bool IsSortable { get; }
            Type Type { get; }
            object GetValue(object o);
        }


        public class TypeReader<T> : ITypeReader {
            private static FieldReader[] _fields;

            static TypeReader() {
                _fields =
                    from p in TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>()
                    where p.PropertyType.IsSerializable == true || p.PropertyType == typeof(XElement)
                    select p;


            }

            public ITypeReader.IFieldReader[] Fields {
                get {
                    return _fields;
                }
            }

            public class FieldReader : ITypeReader.IFieldReader {

                private PropertyDescriptor p;

                public FieldReader(PropertyDescriptor p) {
                    this.p = p;
                }

                public object GetValue(object o) {
                    return p.GetValue(o);
                }

                public string Name {
                    get {
                        return p.Name;
                    }
                }

                public string Title {
                    get {
                        return p.Name.Replace("_", " ");
                    }
                }

                public System.Type Type {
                    get {
                        return p.PropertyType;
                    }
                }

                public bool IsSortable {
                    get {
                        return true;
                    }
                }
            }
        }


    }
}
