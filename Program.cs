using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace LZ78
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("1) Encode\n2) Decode\n3) Exit\nEnter operation number: ");
                if (!Int32.TryParse(Console.ReadLine(), out int operation_number)) continue;
                switch (operation_number)
                {
                    case 1:
                        {
                            Console.Write("Enter file path: ");
                            string file_path = Console.ReadLine();
                            byte[] content = File.ReadAllBytes(file_path);
                            File.WriteAllBytes(file_path, Encode(content));
                            Pause();
                            break;
                        }
                    case 2:
                        {
                            Console.Write("Enter file path: ");
                            string file_path = Console.ReadLine();
                            byte[] content = File.ReadAllBytes(file_path);
                            File.WriteAllBytes(file_path, Decode(content));
                            Pause();
                            break;
                        }
                    case 3:
                        {
                            Environment.Exit(0);
                            break;
                        }
                }
                Console.Clear();
            }
        }

        static void Pause()
        {
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        struct Pair
        {
            public Pair(ushort position, byte next)
            {
                this.position = position;
                this.next = next;
            }
            public ushort position;
            public byte next;
        }

        static byte[] Encode(byte[] content)
        {
            Dictionary<string, ushort> dict = new Dictionary<string, ushort>(){ { "", 0 } };
            List<Pair> pairs = new List<Pair>();
            string prefix = "";
            for(int i = 0; i < content.Length; i++)
            {
                if (dict.ContainsKey(prefix + (char)content[i]))
                {
                    prefix += (char)content[i];
                }
                else
                {
                    if(dict.Count <= 65535) dict.Add(prefix + (char)content[i], (UInt16)dict.Count);
                    pairs.Add(new Pair(dict[prefix], content[i]));
                    prefix = "";
                }
            }
            if(!string.IsNullOrEmpty(prefix))
            {
                byte last_byte = (byte)prefix[prefix.Length - 1];
                prefix = prefix.Substring(0, prefix.Length - 1);
                pairs.Add(new Pair(dict[prefix], last_byte));
            }
            byte[] result = new byte[pairs.Count * 3];
            for(int i = 0, j = 0; i < pairs.Count; i++, j += 3)
            {
                result[j] = (byte)pairs[i].position;
                result[j + 1] = (byte)(pairs[i].position >> 8);
                result[j + 2] = pairs[i].next;
            }
            return result;
        }

        static byte[] Decode(byte[] content)
        {
            List<string> dict = new List<string>() { "" };
            List<byte> result = new List<byte>();
            for (int i = 0; i < content.Length; i += 3)
            {
                ushort position = (UInt16)(content[i] | content[i + 1] << 8);
                result.AddRange((dict[position] + (char)content[i + 2]).Select(c => (byte)c).ToArray());
                if (dict.Count <= 65535) dict.Add(dict[position] + (char)content[i + 2]);
            }
            return result.ToArray();
        }
    }
}
