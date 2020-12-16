using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsumerProducerThreadAssignment
{
    class Bakery
    {
        Queue<string> _spaghettiTray = new Queue<string>(3);
        List<string> freshSpaghetti = new List<string>();

        public void GetSpaghetti()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                lock (_spaghettiTray)
                {
                    while (_spaghettiTray.Count == 0)
                    {
                        Console.WriteLine("Customer is waiting");
                        Monitor.PulseAll(_spaghettiTray);
                        Monitor.Wait(_spaghettiTray);
                    }
                    Console.WriteLine("Customer is consuming " + _spaghettiTray.Dequeue());
                }
                Thread.Sleep(5000);
            }
        }

        public void RefillTray()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                int count = 0;

                if (_spaghettiTray.Count == 0)
                {
                    lock (_spaghettiTray)
                    {
                        if (freshSpaghetti.Count != 0)
                        {

                            foreach (string item in freshSpaghetti)
                            {
                                Console.WriteLine("producer is producing " + item);
                                _spaghettiTray.Enqueue(item);
                            }
                            Monitor.PulseAll(_spaghettiTray);
                        }
                        else if (freshSpaghetti.Count == 0)
                        {
                            while (freshSpaghetti.Count < 3)
                            {
                                count++;
                                string newSpaghet = "Spaghetti " + count.ToString();
                                freshSpaghetti.Add(newSpaghet);
                            }
                        }
                        else if (_spaghettiTray.Count > 1)
                        {
                            Monitor.Wait(_spaghettiTray);
                        }
                    }
                }
                Thread.Sleep(5000);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Bakery bk = new Bakery();

            Thread consumer = new Thread(bk.GetSpaghetti);
            Thread baker = new Thread(bk.RefillTray);

            consumer.Start();
            baker.Start();

            Console.Read();
        }
    }
}
