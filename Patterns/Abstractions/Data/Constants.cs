using System;

namespace Regression
{
    public abstract partial class PatternBase
    {
        #region Constants

        public const string Name = "name";
        public const string Null = "null";

        // TODO: Is it used?
        protected const string MethodName       = "Method";
        protected const string FieldName        = "Field";
        protected const string PropertyName     = "Property";
        protected const string ParameterName    = "value";

        protected const string TDependency      = "TDependency";

        protected const string TEST             = "Testing";
        protected const string OVERRIDE         = "Override";
        protected const string PARAMETER        = "Injecting";
        protected const string CATEGORY_INJECT  = "Injecting";
        protected const string MEMBER_OVERRIDE  = "MemberOverride";
        protected const string VALIDATION       = "Validation";
        protected const string IMPORTING        = "Importing";
        protected const string HIERARCHY        = "Hierarchy";
        protected const string RESOLVING        = "Resolving";
        protected const string ROOT_CONTAINER   = "Root Container";
        protected const string CHILD_CONTAINER  = "Child Container";
        protected const string REGISTERED       = "Registered";
        protected const string UNREGISTERED     = "Unregistered";
        protected const string BUILT_IN         = "Built-In";
        protected const string GENERIC          = "Generic";

        #endregion


        #region Members

        protected const string CONSTRUCTORS = "Constructors";
        protected const string METHODS      = "Methods";
        protected const string FIELDS       = "Fields";
        protected const string PROPERTIES   = "Properties";

        #endregion


        #region Annotations

        protected const string IMPORT_IMPLICIT = "Implicit";
        protected const string IMPORT_REQUIRED = "Required";
        protected const string IMPORT_OPTIONAL = "Optional";
        
        #endregion


        #region Integer

        public const int NamedInt        = 1234;
        public const int DefaultInt      = 3456;
        public const int DefaultValueInt = 4567;
        public const int InjectedInt     = 6789;
        public const int RegisteredInt   = 8901;
        public const int OverriddenInt   = 9012;
        
        #endregion

        
        #region String
        
        public const string NamedString        = "named";
        public const string DefaultString      = "default";
        public const string DefaultValueString = "default_value";
        public const string RegisteredString   = "registered";
        public const string InjectedString     = "injected";
        public const string OverriddenString   = "overridden";
        
        #endregion


        #region Unresolvable
        
        public readonly static Unresolvable NamedUnresolvable        = Unresolvable.Create(NamedString);
        public readonly static Unresolvable DefaultUnresolvable      = Unresolvable.Create(DefaultString);
        public readonly static Unresolvable DefaultValueUnresolvable = Unresolvable.Create(DefaultValueString);
        public readonly static Unresolvable RegisteredUnresolvable   = Unresolvable.Create(RegisteredString);
        public readonly static Unresolvable InjectedUnresolvable     = SubUnresolvable.Create(InjectedString);
        public readonly static Unresolvable OverriddenUnresolvable   = SubUnresolvable.Create(OverriddenString);
        
        #endregion


        #region Struct
        
        public readonly static object RegisteredStruct = new TestStruct(55, "struct");
        public readonly static object NamedStruct      = new TestStruct(44, "named struct");
        
        #endregion


        #region Unresolvable Type Instances
        
        public const bool RegisteredBool              = true;
        public const long RegisteredLong              = 12;
        public const short RegisteredShort            = 23;
        public const float RegisteredFloat            = 34;
        public const double RegisteredDouble          = 45;
        public static Type RegisteredType             = typeof(PatternBase);
        public static ICloneable RegisteredICloneable = new object[0];
        public static Delegate RegisteredDelegate     = (Func<int>)(() => 0);

        #endregion

        
        protected static TestDataSource[] Test_Data_Set = new TestDataSource[]
        {
            #region String

            new TestDataSource<string>(
                    null,                       // default
                    DefaultString,              // DefaultValue
                    DefaultValueString,         // DefaultAttributeValue
                    RegisteredString,           // Registered
                    NamedString,                // Named
                    InjectedString,             // Injected
                    OverriddenString            // Overridden
                ),
                
            #endregion


            #region Integer
#if !BEHAVIOR_V4
            new TestDataSource<int>(
                    0,                          // default
                    DefaultInt,                 // DefaultValue
                    DefaultValueInt,            // DefaultAttributeValue
                    RegisteredInt,              // Registered
                    NamedInt,                   // Named
                    InjectedInt,                // Injected
                    OverriddenInt               // Overridden
                ),
#endif
            #endregion


            #region Unresolvable
                
            new TestDataSource<Unresolvable>(
                    null,                       // default
                    DefaultUnresolvable,        // DefaultValue
                    DefaultValueUnresolvable,   // DefaultAttributeValue
                    RegisteredUnresolvable,     // Registered
                    NamedUnresolvable,          // Named
                    InjectedUnresolvable,       // Injected
                    OverriddenUnresolvable      // Overridden
                ),
            
            #endregion
        };
    }
}


