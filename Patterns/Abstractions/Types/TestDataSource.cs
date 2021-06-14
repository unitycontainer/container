using System;

namespace Regression
{
    public class TestDataSource
    {
        protected object[] _data;

        protected TestDataSource(Type type, object @default,
            object defaultValue, object defaultAttribute,
            object registered, object named,
            object injected, object overridden)
        {
            _data = new object[]
                {
                    type.Name,       
                    type,            
                    defaultValue,    
                    defaultAttribute,
                    registered,      
                    named,           
                    injected,        
                    overridden,      
                    @default,        
                };
        }

        public string Name     => (string)_data[0];
        public Type Type         => (Type)_data[1];

        public object Registered       => _data[4];
        public object Named            => _data[5];
        
        public object Default          => _data[8];
        public object DefaultValue     => _data[2];
        public object DefaultAttribute => _data[3];
        
        public object Injected         => _data[6];
        public object Overridden       => _data[7];

        
        public static explicit operator object[](TestDataSource source) => source._data;
    }

    public class TestDataSource<T> : TestDataSource
    {
        public TestDataSource(object @default, 
            object defaultValue, object defaultAttribute, 
            object registered,   object named, 
            object injected,     object overridden)
            : base(typeof(T), @default, defaultValue, defaultAttribute,
                   registered, named, injected, overridden)
        {
        }
    }
}


