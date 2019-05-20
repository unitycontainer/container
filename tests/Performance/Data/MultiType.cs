using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Runner.Setup;
using Unity;

namespace Runner.Tests
{
    [BenchmarkCategory("Registration")]
    [Config(typeof(BenchmarkConfiguration))]
    public class MultiType
    {
        IUnityContainer _container;
        private static readonly string[] Names = Enumerable.Range(0, 200).Select(i => i.ToString()).ToArray();

        private static readonly Type[] Types =
        {
            new {q11_1 = 1}.GetType(), new {w11_1 = 1}.GetType(), new {e11_1 = 1}.GetType(), new {r11_1 = 1}.GetType(), new {t11_1 = 1}.GetType(), new {y11_1 = 1}.GetType(), new {u11_1 = 1}.GetType(), new {i11_1 = 1}.GetType(),
            new {o11_1 = 1}.GetType(), new {p11_1 = 1}.GetType(), new {a11_1 = 1}.GetType(), new {s11_1 = 1}.GetType(), new {d11_1 = 1}.GetType(), new {f11_1 = 1}.GetType(), new {g11_1 = 1}.GetType(), new {h11_1 = 1}.GetType(),
            new {j11_1 = 1}.GetType(), new {k11_1 = 1}.GetType(), new {l11_1 = 1}.GetType(), new {z11_1 = 1}.GetType(), new {x11_1 = 1}.GetType(), new {c11_1 = 1}.GetType(), new {v11_1 = 1}.GetType(), new {b11_1 = 1}.GetType(),
            new {n11_1 = 1}.GetType(), new {m11_1 = 1}.GetType(), new {q21_1 = 1}.GetType(), new {w21_1 = 1}.GetType(), new {e21_1 = 1}.GetType(), new {r21_1 = 1}.GetType(), new {t21_1 = 1}.GetType(), new {y21_1 = 1}.GetType(),
            new {u21_1 = 1}.GetType(), new {i21_1 = 1}.GetType(), new {o21_1 = 1}.GetType(), new {p21_1 = 1}.GetType(), new {a21_1 = 1}.GetType(), new {s21_1 = 1}.GetType(), new {d21_1 = 1}.GetType(), new {f21_1 = 1}.GetType(),
            new {g21_1 = 1}.GetType(), new {h21_1 = 1}.GetType(), new {j21_1 = 1}.GetType(), new {k21_1 = 1}.GetType(), new {l21_1 = 1}.GetType(), new {z21_1 = 1}.GetType(), new {x21_1 = 1}.GetType(), new {c21_1 = 1}.GetType(),
            new {v21_1 = 1}.GetType(), new {b21_1 = 1}.GetType(), new {n21_1 = 1}.GetType(), new {m21_1 = 1}.GetType(), new {q12_1 = 1}.GetType(), new {w12_1 = 1}.GetType(), new {e12_1 = 1}.GetType(), new {r12_1 = 1}.GetType(),
            new {t12_1 = 1}.GetType(), new {y12_1 = 1}.GetType(), new {u12_1 = 1}.GetType(), new {i12_1 = 1}.GetType(), new {o12_1 = 1}.GetType(), new {p12_1 = 1}.GetType(), new {a12_1 = 1}.GetType(), new {s12_1 = 1}.GetType(),
            new {d12_1 = 1}.GetType(), new {f12_1 = 1}.GetType(), new {g12_1 = 1}.GetType(), new {h12_1 = 1}.GetType(), new {j12_1 = 1}.GetType(), new {k12_1 = 1}.GetType(), new {l12_1 = 1}.GetType(), new {z12_1 = 1}.GetType(),
            new {x12_1 = 1}.GetType(), new {c12_1 = 1}.GetType(), new {v12_1 = 1}.GetType(), new {b12_1 = 1}.GetType(), new {n12_1 = 1}.GetType(), new {m12_1 = 1}.GetType(), new {q22_1 = 1}.GetType(), new {w22_1 = 1}.GetType(),
            new {e22_1 = 1}.GetType(), new {r22_1 = 1}.GetType(), new {t22_1 = 1}.GetType(), new {y22_1 = 1}.GetType(), new {u22_1 = 1}.GetType(), new {i22_1 = 1}.GetType(), new {o22_1 = 1}.GetType(), new {p22_1 = 1}.GetType(),
            new {a22_1 = 1}.GetType(), new {s22_1 = 1}.GetType(), new {d22_1 = 1}.GetType(), new {f22_1 = 1}.GetType(), new {g22_1 = 1}.GetType(), new {h22_1 = 1}.GetType(), new {j22_1 = 1}.GetType(), new {k22_1 = 1}.GetType(),
            new {l22_1 = 1}.GetType(), new {z22_1 = 1}.GetType(), new {x22_1 = 1}.GetType(), new {c22_1 = 1}.GetType(), new {v22_1 = 1}.GetType(), new {b22_1 = 1}.GetType(), new {n22_1 = 1}.GetType(), new {m22_1 = 1}.GetType(),
            new {q11_2 = 1}.GetType(), new {w11_2 = 1}.GetType(), new {e11_2 = 1}.GetType(), new {r11_2 = 1}.GetType(), new {t11_2 = 1}.GetType(), new {y11_2 = 1}.GetType(), new {u11_2 = 1}.GetType(), new {i11_2 = 1}.GetType(),
            new {o11_2 = 1}.GetType(), new {p11_2 = 1}.GetType(), new {a11_2 = 1}.GetType(), new {s11_2 = 1}.GetType(), new {d11_2 = 1}.GetType(), new {f11_2 = 1}.GetType(), new {g11_2 = 1}.GetType(), new {h11_2 = 1}.GetType(),
            new {j11_2 = 1}.GetType(), new {k11_2 = 1}.GetType(), new {l11_2 = 1}.GetType(), new {z11_2 = 1}.GetType(), new {x11_2 = 1}.GetType(), new {c11_2 = 1}.GetType(), new {v11_2 = 1}.GetType(), new {b11_2 = 1}.GetType(),
            new {n11_2 = 1}.GetType(), new {m11_2 = 1}.GetType(), new {q21_2 = 1}.GetType(), new {w21_2 = 1}.GetType(), new {e21_2 = 1}.GetType(), new {r21_2 = 1}.GetType(), new {t21_2 = 1}.GetType(), new {y21_2 = 1}.GetType(),
            new {u21_2 = 1}.GetType(), new {i21_2 = 1}.GetType(), new {o21_2 = 1}.GetType(), new {p21_2 = 1}.GetType(), new {a21_2 = 1}.GetType(), new {s21_2 = 1}.GetType(), new {d21_2 = 1}.GetType(), new {f21_2 = 1}.GetType(),
            new {g21_2 = 1}.GetType(), new {h21_2 = 1}.GetType(), new {j21_2 = 1}.GetType(), new {k21_2 = 1}.GetType(), new {l21_2 = 1}.GetType(), new {z21_2 = 1}.GetType(), new {x21_2 = 1}.GetType(), new {c21_2 = 1}.GetType(),
            new {v21_2 = 1}.GetType(), new {b21_2 = 1}.GetType(), new {n21_2 = 1}.GetType(), new {m21_2 = 1}.GetType(), new {q12_2 = 1}.GetType(), new {w12_2 = 1}.GetType(), new {e12_2 = 1}.GetType(), new {r12_2 = 1}.GetType(),
            new {t12_2 = 1}.GetType(), new {y12_2 = 1}.GetType(), new {u12_2 = 1}.GetType(), new {i12_2 = 1}.GetType(), new {o12_2 = 1}.GetType(), new {p12_2 = 1}.GetType(), new {a12_2 = 1}.GetType(), new {s12_2 = 1}.GetType(),
            new {d12_2 = 1}.GetType(), new {f12_2 = 1}.GetType(), new {g12_2 = 1}.GetType(), new {h12_2 = 1}.GetType(), new {j12_2 = 1}.GetType(), new {k12_2 = 1}.GetType(), new {l12_2 = 1}.GetType(), new {z12_2 = 1}.GetType(),
            new {x12_2 = 1}.GetType(), new {c12_2 = 1}.GetType(), new {v12_2 = 1}.GetType(), new {b12_2 = 1}.GetType(), new {n12_2 = 1}.GetType(), new {m12_2 = 1}.GetType(), new {q22_2 = 1}.GetType(), new {w22_2 = 1}.GetType(),
            new {e22_2 = 1}.GetType(), new {r22_2 = 1}.GetType(), new {t22_2 = 1}.GetType(), new {y22_2 = 1}.GetType(), new {u22_2 = 1}.GetType(), new {i22_2 = 1}.GetType(), new {o22_2 = 1}.GetType(), new {p22_2 = 1}.GetType(),
            new {a22_2 = 1}.GetType(), new {s22_2 = 1}.GetType(), new {d22_2 = 1}.GetType(), new {f22_2 = 1}.GetType(), new {g22_2 = 1}.GetType(), new {h22_2 = 1}.GetType(), new {j22_2 = 1}.GetType(), new {k22_2 = 1}.GetType(),
            new {l22_2 = 1}.GetType(), new {z22_2 = 1}.GetType(), new {x22_2 = 1}.GetType(), new {c22_2 = 1}.GetType(), new {v22_2 = 1}.GetType(), new {b22_2 = 1}.GetType(), new {n22_2 = 1}.GetType(), new {m22_2 = 1}.GetType()
        };


        [IterationSetup]
        public virtual void SetupContainer()
        {
            _container = new UnityContainer();
            _container.RegisterType<Poco>();
            _container.RegisterType<IService, Service>();
            _container.RegisterType<IService, Service>("1");
            _container.RegisterType<IService, Service>("2");
        }


        public interface IService { }
        public class Service : IService { }
        public class Poco { }
    }
}

