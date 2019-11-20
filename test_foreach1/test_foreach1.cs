using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace foreach_test1
{
    // 尖括号中的类型参数 T。
    public class MyList<T> : IEnumerable<T>
    {
        protected Node head;
        protected Node current = null;

        // 嵌套类型也是 T 上的泛型
        protected class Node
        {
            public Node next;
            // T 作为私有成员数据类型。
            private T data;
            // 在非泛型构造函数中使用的 T。
            public Node(T t)
            {
                next = null;
                data = t;
            }
            public Node Next
            {
                get { return next; }
                set { next = value; }
            }
            // T 作为属性的返回类型。
            public T Data
            {
                get { return data; }
                set { data = value; }
            }
        }

        public MyList()
        {
            head = null;
        }

        // T 作为方法参数类型。
        public void AddHead(T t)
        {
            Node n = new Node(t);
            n.Next = head;
            head = n;
        }

        // 实现 GetEnumerator 以返回 IEnumerator<T>，从而启用列表的
        // foreach 迭代。请注意，在 C# 2.0 中， 
        // 不需要实现 Current 和 MoveNext。
        // 编译器将创建实现 IEnumerator<T> 的类。
        public IEnumerator<T> GetEnumerator()
        {
            Node current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        // 必须实现此方法，因为
        // IEnumerable<T> 继承 IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    // 一个将自身作为类型参数来实现 IComparable<T> 的简单类，
    // 是对象中的
    // 常用设计模式，这些对象
    // 存储在泛型列表中。
    public class Person
    {
        string name;
        int age;

        public Person(string s, int i)
        {
            name = s;
            age = i;
        }

        public override string ToString()
        {
            return name + ":" + age;
        }

    }

    public class Dog
    {
        string name;
        int age;
        int sex;

        public Dog(String name, int age, int sex)
        {
            this.name = name;
            this.age = age;
            this.sex = sex;
        }

        public override string ToString()
        {
            return "{" + name + " " + age + " " + sex + "}";
        }
    }

    class Generics
    {
        static void Main(string[] args)
        {
            // Person 是类型参数。
            MyList<Person> plist = new MyList<Person>();
            MyList<Dog> dlist = new MyList<Dog>();

            // 创建 name 和 age 值以初始化 Person 对象。
            string[] names = new string[] { "Franscoise", "Bill", "Li", "Sandra", "Gunnar", "Alok", "Hiroyuki", "Maria", "Alessandro", "Raul" };
            int[] ages = new int[] { 45, 19, 28, 23, 18, 9, 108, 72, 30, 35 };
            int[] sexes = new int[] { 1, 1, 0, 1, 0, 1, 0, 0, 1, 0 };
            // 填充列表。
            for (int x = 0; x < names.Length; x++)
            {
                plist.AddHead(new Person(names[x], ages[x]));
                dlist.AddHead(new Dog(names[x], ages[x], sexes[x]));
            }
            foreach (Person p in plist)
            {
                Console.WriteLine(p.ToString());
            }
            Console.WriteLine("----------");
            foreach (Dog d in dlist)
            {
                Console.WriteLine(d.ToString());
            }


            Console.ReadKey();
        }
    }

}
