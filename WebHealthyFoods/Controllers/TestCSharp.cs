using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Speech.Recognition;
using System.Threading;
using static System.Math;
using M = System.Math;

// TODO: <---MAKE Disable the "Allow unsafe code" flag in project settings !!!

namespace WebHealthyFoods.Controllers
{
    //public class TestBase
    //{
    //    public virtual string TestMsg(string message)
    //    {
    //        return $"Base message: {message}";
    //    }
    //}

    //public class Test : TestBase
    //{
    //    private string rembered;

    //    public string input = "DEFAULT";

    //    private string Input
    //    {
    //        get
    //        {
    //            return input;
    //        }
    //        set
    //        {
    //            if (value == null)
    //            {
    //                input = "UNKNOWN";
    //            }
    //            else
    //            {
    //                input = value;
    //            }
    //        }
    //    }

    //    private string SetInput(string value)
    //    {
    //        return $"##{value}##";
    //    }

    //    private string GetInput()
    //    {
    //        return Input;
    //    }

    //    public Test()
    //    {
    //    }

    //    public Test(string input)
    //    {
    //        this.Input = input;
    //    }

    //    public override string ToString()
    //    {
    //        return this.Input;
    //    }

    //    public override string TestMsg(string message)
    //    {
    //        string baseTxt = base.TestMsg("BASE_MESSAGE1");
    //        string txt = $"""
    //            There is a message
    //            sent as parameter
    //            with content described
    //            on the next line:
    //            {message}
    //            """;
    //        return baseTxt + "\r\n" + txt + "\r\n" + this.Input;
    //    }
    //}

    //public class TestChild : Test
    //{

    //    private string input = "UNKNOWN";

    //    public TestChild(string input) : base(input)
    //    {
    //        this.input = $"CHILD*{input}*CHILD";
    //    }

    //    public override string TestMsg(string message)
    //    {
    //        return base.TestMsg(message) + "\r\n" + this.input;
    //    }

    //    public string SimpleMsg(string input)
    //    {
    //        return input;
    //    }
    //}

    //public class TestCSharp
    //{

    //    public delegate string TestMsgLambda(string input);

    //    public event TestMsgLambda TestMsgEvent1;
    //    public event TestMsgLambda TestMsgEvent2;

    //    public delegate string Test2Delegate(int a, params object[] parameters);
    //    public event Test2Delegate Test2Event;

    //    private static int counter = 1;

    //    private string Test2(int a, params object[] parameters)
    //    {
    //        string txt = "";
    //        foreach (var param in parameters)
    //        {
    //            if (txt != "")
    //            {
    //                txt = txt + "; ";
    //            }
    //            txt += param.ToString();
    //        }
    //        return $"{counter++} {a} " + txt;
    //    }

    //    private static string TestStaticMsg(string input)
    //    {
    //        return $"STATIC {input}";
    //    }

    //    private async Task TestAsync(string input)
    //    {
    //        await Task.Run(() =>
    //        {
    //            string msg = "test";
    //        });
    //        Debug.WriteLine("BREAK");
    //    }

    //    private async Task<string> TestAsync2(string input)
    //    {
    //        TestChild o = new TestChild("ASYNC TEST2");
    //        string result = await Task.FromResult(o.TestMsg("FROM_RESULT"));
    //        return result;
    //    }

    //    private async Task<string> TestAsync3(string input)
    //    {
    //        TestChild o = new TestChild("ASYNC TEST3");
    //        Task<string> task1 = Task.Run(() =>
    //        {
    //            return o.SimpleMsg("TASK1");
    //        });
    //        Task<string> task2 = Task.Run(() =>
    //        {
    //            return o.SimpleMsg("TASK2");
    //        });
    //        string[] resultsAwait = await Task.WhenAll(task1, task2);
    //        string result = string.Join("\r\n-------------------------\r\n", resultsAwait);
    //        return result;
    //    }

    //    private async Task TestAsync4(string input)
    //    {
    //        await Task.FromException(new Exception("Test exception"));
    //    }

    //    public async Task TestMethod()
    //    {
    //        try
    //        {
    //            Debug.WriteLine("--------------------------------------");

    //            // Testovani trid.
    //            object obj = new TestChild("TEST");
    //            TestBase a = obj as Test;
    //            string str = a.ToString() + $" object {a.TestMsg("MESSAGE1")}";
    //            Debug.WriteLine(str);

    //            // Testovani "checked/unchecked".
    //            int a1 = int.MaxValue;
    //            try
    //            {
    //                int b = checked(a1 + 1);
    //                Debug.WriteLine($"b = {b}");
    //            }
    //            catch (Exception ex)
    //            {
    //                Debug.WriteLine(ex.Message);
    //            }

    //            // Testovani "delegate".
    //            Test c = new Test();
    //            TestMsgLambda lambda = c.TestMsg;
    //            string d = lambda("Testing lambda OK");
    //            Debug.WriteLine($"d = {d}");
    //            lambda = TestStaticMsg;
    //            string e = lambda("Testing lambda OK");
    //            Debug.WriteLine($"e = {e}");
    //            EventHandler handler = delegate
    //            {
    //                int a = 1;
    //            };
    //            handler.Invoke(this, null);

    //            // Testovani "events".
    //            TestMsgEvent1 += a.TestMsg;
    //            TestMsgEvent1.BeginInvoke("Posted message", (result) =>
    //            {

    //                var asyncResult = (System.Runtime.Remoting.Messaging.AsyncResult)result;
    //                var del = (TestMsgLambda)asyncResult.AsyncDelegate;
    //                string @string = del.EndInvoke(result);
    //                Debug.WriteLine(@string);
    //                Debug.WriteLine("^^^^^^^^^^^^^^^EVENT RESULT^^^^^^^^^^^");

    //            }, null);
    //            TestMsgEvent2 += a.TestMsg;
    //            TestMsgEvent2 += lambda;
    //            string f = TestMsgEvent2.Invoke("Sent message");
    //            Debug.WriteLine($"f = {f}");
    //            Test2Event += Test2;
    //            Test2Event += Test2;
    //            Test2Event += Test2;
    //            string str2 = Test2Event.Invoke(1, "hello", 3.14, 'A');

    //            // Testovani "await".
    //            await TestAsync("AWAIT1");
    //            string waitResult = await TestAsync2("AWAIT2");
    //            Debug.WriteLine($"waitResult2 = {waitResult}");
    //            waitResult = await TestAsync3("AWAIT3");
    //            Debug.WriteLine($"waitResult3 = {waitResult}");
    //            try
    //            {
    //                await TestAsync4("AWAIT4");
    //            }
    //            catch (Exception ex1)
    //            {
    //                Debug.WriteLine(ex1.Message);
    //            }

    //            // Testovani "operator".
    //            Complex c1 = new Complex(null, 1f);
    //            Complex c2 = new Complex(1f, null);
    //            Debug.WriteLine($"c1 = {c1} = {(Exp)c1}\r\nc2 = {c2} = {(Exp)c2}");
    //            Complex c3 = c1 + c2;
    //            Debug.WriteLine($"c3 = {c1} + {c2} = {c3} = {(Exp)c3}");
    //            c3 = c1 * (-c2);
    //            Debug.WriteLine($"c3 = ({c1}) * -({c2}) = {c3} = {(Exp)c3}");

    //            // Testovani "extension".
    //            string s1 = "abcdef".Mul("123456");
    //            Debug.WriteLine($"s1 =\r\n{s1}");

    //            // Testovani "implicit", "explicit".
    //            Exp imp = c1;
    //            Debug.WriteLine(imp);

    //            // Testovani "extern".
    //            string input = "HELLO WORLD! (ěščřžýáíéĚŠČŘŽÝÁÍÉ)";
    //            string lowered = Marshal.PtrToStringUni(CharLowerW(input));
    //            Debug.WriteLine(lowered);

    //            // Testovani "unsafe" "fixed".
    //            unsafe
    //            {
    //                fixed (char* p = lowered)
    //                {
    //                    for (int ci = 0; ; ci++)
    //                    {
    //                        char* cp = p + ci;
    //                        if (*cp == 0)
    //                        {
    //                            Debug.WriteLine("");
    //                            break;
    //                        }
    //                        Debug.Write((ci != 0 ? " " : "") + $"c[{ci}]='{*cp}'");
    //                    }
    //                }
    //                int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8 };
    //                fixed (int* p1 = &numbers[3], p2 = &numbers[4])
    //                {
    //                    Debug.WriteLine($"p1 = {*p1}, p2 = {*p2}");
    //                }
    //                UnsafeStruct strct1 = new UnsafeStruct(1, "AAA", null);
    //                UnsafeStruct strct2 = new UnsafeStruct(2, "BBB", &strct1);
    //                UnsafeStruct pStrct = *strct2.pNode;
    //                string txt = pStrct.str;
    //                Debug.WriteLine($"txt = {txt}");
    //                pStrct.SetNode(&strct2);
    //                txt = pStrct.pNode->str;
    //                Debug.WriteLine($"txt = {txt}");
    //                Debug.WriteLine($"pStrct.str = {pStrct.str}");
    //            }

    //            // Testovani "stackalloc".
    //            unsafe
    //            {
    //                int[] nArr = new int[20];
    //                fixed (int* pArr = nArr)
    //                {
    //                    int* p1 = pArr;
    //                    int* p2 = stackalloc int[20];
    //                    Debug.WriteLine($"p1 = 0x{((Int64)p1):x16}");
    //                    Debug.WriteLine($"p2 = 0x{((Int64)p2):x16}");
    //                }
    //            }

    //            // Testovani "internal".
    //            // Following function is available only in current assembly.
    //            int n = GetNumber(15);
    //            Debug.WriteLine($"n = {n}");
    //            float f2 = ClassInAssembly.GetFloat("3.14");
    //            Debug.WriteLine($"f2 = {f2}");

    //            // Testovani "is".
    //            // TODO: <---MAKE Projit si znovu.
    //            obj = new TestBase();
    //            bool success = obj is TestBase;
    //            Debug.WriteLine($"success = {success}");
    //            success = obj is Test;
    //            Debug.WriteLine($"success = {success}");
    //            obj = null;
    //            success = obj is null;
    //            Debug.WriteLine($"success = {success}");
    //            obj = new Test();
    //            success = obj is not null;
    //            Debug.WriteLine($"success = {success}");
    //            int num = 37;
    //            success = (num is int num2);
    //            Debug.WriteLine($"success = {success}, num2 = {num2}");
    //            var rec1 = new { count = 10, msg = "Hello", flag = true };
    //            Debug.WriteLine($"rec1 = {rec1}");
    //            success = rec1 is { count: 10 };
    //            Debug.WriteLine($"success = {success}");
    //            success = rec1 is { count: > 10 };
    //            Debug.WriteLine($"success = {success}");
    //            string MapNum(int n) => n switch
    //            {
    //                1 => "a",
    //                2 => "l",
    //                3 => "f",
    //                > 5 and < 10 => "p",
    //                _ => "DEF"
    //            };
    //            str = MapNum(1) + MapNum(3) + MapNum(2) + MapNum(10) + MapNum(7);
    //            Debug.WriteLine(str);
    //            bool CheckFloat(float f) => f is (>= 1.0f and <= 2.0f);
    //            success = CheckFloat(1.2f);
    //            Debug.WriteLine($"success = {success}");
    //            success = CheckFloat(0.3f);
    //            Debug.WriteLine($"success = {success}");

    //            // Testovani "local function".
    //            string GetTextStr(float f)
    //            {
    //                string str = $"#{f}";
    //                return str;
    //            }
    //            float GetPI() => (float)Math.PI;
    //            str = GetTextStr(15f * GetPI());
    //            Debug.WriteLine(str);

    //            // Testovani "lock".
    //            FileStream fs = File.OpenRead(@"C:\test\test.txt");
    //            lock (fs)
    //            {
    //                int len = 256;
    //                byte[] bytes = new byte[len];
    //                int bytesRead = fs.Read(bytes, 0, len);
    //                fs.Close();
    //                Debug.WriteLine("BYTES:\r\n");
    //                string s = "";
    //                foreach (byte b in bytes) s = s + b + ' ';
    //                Debug.WriteLine(s);
    //                Debug.WriteLine($"bytesRead = {bytesRead}");
    //            }

    //            // Testovani "param".
    //            string JoinParams(params object[] parameters)
    //            {
    //                string s = "";
    //                string div = "_";
    //                foreach (object parameter in parameters)
    //                {
    //                    s = s + (s != "" ? div : "") + (parameter != null ? parameter.ToString() : "null");
    //                }
    //                return s;
    //            }
    //            str = JoinParams(3.14f, 2, 'B', "Hello world", true, null);
    //            Debug.WriteLine(str);

    //            // Testovani "volatile".
    //            Thread thread1 = new Thread(() =>
    //            {
    //                strVol = "HELLO";
    //                Debug.WriteLine(strVol);
    //            });
    //            Thread thread2 = new Thread(() =>
    //            {
    //                strVol = "HI THERE";
    //                Debug.WriteLine(strVol);
    //            });
    //            thread1.Start();
    //            thread2.Start();

    //            // Testovani "ref".
    //            bool LoadText(ref string text)
    //            {
    //                text = "RETURNED TEXT";
    //                return true;
    //            }
    //            success = LoadText(ref str);
    //            Debug.WriteLine($"success = {success}, str = {str}");
    //            int n2 = 234;
    //            ref int n3 = ref n2;
    //            n2 = 432;
    //            Debug.WriteLine(n3);
    //            StackStruct st;
    //            st.Name = "Structure on stack";
    //            st.Count = 3;
    //            Debug.WriteLine($"s = ({st.Name}, {st.Count})");

    //            // Testovani "templates".
    //            MyClass1<ClassInAssembly> cls1 = new()
    //            {
    //                value = new()
    //            };
    //            Debug.WriteLine(cls1.value.Name);

    //            // Testovani "using".
    //            string path = @"C:\test\test.txt";
    //            using (StreamReader sr = new StreamReader(path))
    //            {
    //                string fileContents = sr.ReadToEnd();
    //                Debug.WriteLine(fileContents);
    //            }
    //            {
    //                using StreamReader sr1 = new StreamReader(path);
    //                string fileContents = sr1.ReadLine();
    //                Debug.WriteLine(fileContents);
    //            }
    //            StreamReader sr2 = File.OpenText(path);
    //            using (sr2)
    //            {
    //                string fileContents = sr2.ReadLine();
    //                Debug.WriteLine(fileContents);
    //            }
    //            try
    //            {
    //                str = sr2.ReadLine();
    //            }
    //            catch (Exception e2)
    //            {
    //                Debug.WriteLine(e2.Message);
    //            }
    //            double pi = PI;         // Pridano: using static System.Math;
    //            Debug.WriteLine(pi);
    //            double eul = M.E;       // Pridano: using M = System.Math;
    //            Debug.WriteLine(eul);

    //            // Testovani "add, remove".
    //            Test o2 = new Test();
    //            TestEvent += o2.TestMsg;
    //            string res1 = _TestEvent.Invoke("INVOKE EVENT");
    //            Debug.WriteLine($"res1 = {res1}");
    //            TestEvent -= o2.TestMsg;

    //            // Testovani "LINQ".
    //            int[] numbers2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    //            IQueryable<int> query = numbers2.AsQueryable();
    //            var evenNum = from n5 in query
    //                          where n5 % 2 != 0
    //                          select n5;
    //            foreach (int n6 in evenNum)
    //            {
    //                Debug.Write(n6);
    //            }
    //            Debug.WriteLine("");
    //            dynamic[] recs3 = {
    //                new { a = 1, b = "A"},
    //                new { a = 2, b = "B"},
    //                new { a = 3, b = "C"},
    //                new { a = 4, b = "D"},
    //                new { a = 5, b = "E"},
    //                new { a = 6, b = "F"},
    //                new { a = 7, b = "G"},
    //                new { a = 8, b = "H"},
    //                new { a = 9, b = "I"},
    //                new { a = 10, b = "J"}
    //            };
    //            var filt1 = from r1 in recs3
    //                        where (r1.a % 2 == 0 && r1.a <= 7)
    //                        select r1.b;
    //            foreach (string s2 in filt1)
    //            {
    //                Debug.Write(s2);
    //            }
    //            Debug.WriteLine("");
    //            dynamic[] table1 = [
    //                new { n = 2, s="abcd" },
    //                new { n = 5, s="xyz" },
    //                new { n = 3, s="12345" },
    //                new { n = 7, s="hello" },
    //                new { n = 4, s="world" },
    //                new { n = 7, s="hi" },
    //                new { n = 7, s="hi there" },
    //                new { n = 5, s="rstu" },
    //                ];
    //            var filtered2 = from item in table1
    //                            where item.n > 3
    //                            group item by item.n into g
    //                            select new { key = g.Key, count = g.Count() };
    //            foreach (var item5 in filtered2)
    //            {
    //                Debug.WriteLine($"id={item5.key}, count={item5.count}");
    //            }
    //            var res6 = from t1 in table1
    //                       join t2 in recs3 on t1.n equals t2.a
    //                       select new { n = t1.n, t1 = t1.s, t2 = t2.b };
    //            foreach (var item7 in res6)
    //            {
    //                Debug.WriteLine($"n={item7.n}, t1={item7.t1}, t2={item7.t2}");
    //            }

    //            // Testovani "INITIALIZER".
    //            Plant plant1 = new Plant { Id = 1, Name = "Tree", Origin = { Id = 2, Location = "Valencia" } };
    //            Debug.WriteLine(plant1);
    //            List<int> list3 = new List<int> { 1, 2, 7, 2, 6, 8, 8, 5, 3, 35 };
    //            foreach (int n7 in list3)
    //            {
    //                Debug.Write(n7 + " ");
    //            }
    //            Debug.WriteLine("");
    //            List<int> list4 = new() { 52, 58, 69, 95, 43, 51, 45, 66 };
    //            List<int> list5 = [.. list4, .. list3];
    //            foreach (int n8 in list5)
    //            {
    //                Debug.Write(n8 + " ");
    //            }
    //            Debug.WriteLine("");
    //            var items = new Dictionary<string, string>
    //            {
    //                ["Abcd"] = "55_eeee",
    //                ["Xyz"] = "77_ffff",
    //                ["Hello"] = "99_gggg"
    //            };
    //            foreach (var item in items)
    //            {
    //                Debug.WriteLine($"key={item.Key}, value={item.Value}");
    //            }

    //            var items3 = new Dictionary<int, string>
    //            {
    //                [8] = "55_eeee",
    //                [12] = "77_ffff",
    //                [5] = "99_gggg"
    //            };
    //            foreach (var item in items3)
    //            {
    //                Debug.WriteLine($"key={item.Key}, value={item.Value}");
    //            }

    //            // Testovani "dynamic".
    //            dynamic dyn1 = 3.14;
    //            Debug.WriteLine($"dyn1={dyn1}, dyn1 type={dyn1.GetType()}");

    //            Debug.WriteLine("--------------------------------------");

    //            // TODO: <---TEST Speech to text.
    //            Speech();
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.Write(ex.Message);
    //        }
    //    }

    //    private class Plant
    //    {
    //        public int Id { get; set; }
    //        public string Name { get; set; }
    //        public Origin Origin { get; set; } = new();
    //        public string Desc => $"Plant {Name} from {Origin.Location}";

    //        public override string ToString() => Desc;
    //    }

    //    private class Origin
    //    {
    //        public int Id { get; set; }
    //        public string Location { set; get; }

    //        public override string ToString() => $"Id={Id}, Location={Location}";
    //    }

    //    private string TestCSharp_TestEvent2(string sender)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private delegate string TestDelegater(string sender);
    //    private TestDelegater _TestEvent;
    //    private event TestDelegater TestEvent
    //    {
    //        add
    //        {
    //            _TestEvent += value;
    //            Debug.WriteLine($"Adding event handler {value.Method.Name}");
    //        }
    //        remove
    //        {
    //            _TestEvent -= value;
    //            Debug.WriteLine($"Removing event handler {value.Method.Name}");
    //        }
    //    }

    //    internal class MyClass1<T> where T : ClassInAssembly
    //    {
    //        public T value = null;
    //    }

    //    ref struct StackStruct
    //    {
    //        public string Name;
    //        public int Count;
    //    }

    //    static void Speech()
    //    {
    //        foreach (var x in SpeechRecognitionEngine.InstalledRecognizers())
    //        {
    //            Debug.WriteLine(x.Culture.Name);
    //        }

    //        var recognizer = new SpeechRecognitionEngine(new CultureInfo("en-US"));
    //        recognizer.LoadGrammar(new DictationGrammar());

    //        recognizer.SpeechRecognized += (s, e) =>
    //        {
    //            Debug.WriteLine("Rozpoznáno: " + e.Result.Text);
    //        };

    //        recognizer.SetInputToDefaultAudioDevice();
    //        recognizer.RecognizeAsync(RecognizeMode.Multiple);

    //        Debug.WriteLine("Mluvte česky...");
    //    }

    //    volatile String strVol = "VOLATILE";

    //    record Point
    //    {
    //        public int X = 0;
    //        public int Y = 0;
    //    }

    //    record Bounds
    //    {
    //        public Point LeftTop;
    //        public Point RightBottom;
    //    }

    //    internal int GetNumber(int n)
    //    {
    //        int m = n % 12;
    //        return m;
    //    }

    //    internal class ClassInAssembly
    //    {
    //        public string Name = "MY NAME 1";

    //        public static float GetFloat(string input)
    //        {
    //            bool success = float.TryParse(input, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float f);
    //            if (!success)
    //            {
    //                return 0f;
    //            }
    //            return f;
    //        }
    //    }

    //    public unsafe struct UnsafeStruct
    //    {
    //        public int n = 3;
    //        public string str = "Hello world!";
    //        public unsafe UnsafeStruct* pNode = null;

    //        public UnsafeStruct(int n, string str, UnsafeStruct* pNode)
    //        {
    //            this.n = n;
    //            this.str = str;
    //            this.pNode = pNode;
    //        }

    //        public unsafe void SetNode(UnsafeStruct* pNode)
    //        {
    //            string constStr = "CONSTANT_STRING";
    //            string* pStr = &constStr;
    //            void* ptr = pStr;
    //            ptr = (void*)0x1111;
    //            this.pNode = pNode;
    //            this.str = *pStr;
    //        }
    //    }

    //    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    //    private static extern IntPtr CharLowerW(string str);

    //    public class Exp
    //    {
    //        public float A = 0f;
    //        public float phi = 0f;

    //        public static implicit operator Exp(Complex c)
    //        {
    //            Exp exp = new Exp(c);
    //            return exp;
    //        }

    //        /*public static explicit operator Exp(Complex c)
    //        {
    //            Exp exp = new Exp(c);
    //            return exp;
    //        }*/

    //        public Exp(Complex c)
    //        {
    //            this.A = (float)Math.Sqrt(Math.Abs(c.a * c.a) + Math.Abs(c.b * c.b));
    //            this.phi = (float)(Math.Acos(c.a / this.A) % (2 * Math.PI));
    //        }

    //        public override string ToString()
    //        {
    //            string exponent;
    //            if ((phi % 2 * Math.PI) == 0f)
    //            {
    //                return $"{A}";
    //            }
    //            exponent = $"e^i{phi}";
    //            if (A == 1f)
    //            {
    //                return exponent;
    //            }
    //            else if (A == 0f)
    //            {
    //                return "0";
    //            }
    //            return $"{A}{exponent}";
    //        }
    //    }

    //    public class Complex
    //    {
    //        public float a = 0f;
    //        public float b = 0f;

    //        public Complex(float? a, float? b)
    //        {
    //            this.a = a ?? 0.0f;
    //            this.b = b ?? 0.0f;
    //        }

    //        public static Complex operator +(Complex c1, Complex c2)
    //        {
    //            Complex c3 = new Complex(c1.a + c2.a, c1.b + c2.b);
    //            return c3;
    //        }

    //        public static Complex operator *(Complex c1, Complex c2)
    //        {
    //            Complex c3 = new Complex(c1.a * c2.a - c1.b * c2.b, c1.a * c2.b + c1.b * c2.a);
    //            return c3;
    //        }

    //        public static Complex operator -(Complex c1)
    //        {
    //            Complex c2 = new Complex(-c1.a, -c1.b);
    //            return c2;
    //        }

    //        public override string ToString()
    //        {
    //            return $"{a} + {b}i";
    //        }
    //    }
    //}

    //public static class Extension
    //{
    //    public static string Mul(this string a, string b)
    //    {
    //        int la = a.Length;
    //        int lb = b.Length;
    //        string s = "";
    //        for (int i = 0; i < la; i++)
    //        {
    //            char ca = a[i];
    //            for (int j = 0; j < lb; j++)
    //            {
    //                char cb = b[j];
    //                s += $"{ca}{cb}" + (j < lb - 1 ? " " : "");
    //            }
    //            s += (i < la - 1 ? "\r\n" : "");
    //        }
    //        return s;
    //    }
    //}
}