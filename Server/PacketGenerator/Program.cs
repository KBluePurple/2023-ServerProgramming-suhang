using System.IO;

namespace PacketGenerator
{
    internal class Program
    {
        private static string clientRegister;
        private static string serverRegister;

        private static void Main(string[] args)
        {
            var file = "../../../Common/protoc-3.12.3-win64/bin/Protocol.proto";
            if (args.Length >= 1)
                file = args[0];

            var startParsing = false;
            foreach (var line in File.ReadAllLines(file))
            {
                if (!startParsing && line.Contains("enum MsgId"))
                {
                    startParsing = true;
                    continue;
                }

                if (!startParsing)
                    continue;

                if (line.Contains("}"))
                    break;

                var names = line.Trim().Split(" =");
                if (names.Length == 0)
                    continue;

                var name = names[0];
                if (name.StartsWith("S_"))
                {
                    var words = name.Split("_");

                    var msgName = "";
                    foreach (var word in words)
                        msgName += FirstCharToUpper(word);

                    var packetName = $"S_{msgName.Substring(1)}";
                    clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
                }
                else if (name.StartsWith("C_"))
                {
                    var words = name.Split("_");

                    var msgName = "";
                    foreach (var word in words)
                        msgName += FirstCharToUpper(word);

                    var packetName = $"C_{msgName.Substring(1)}";
                    serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
                }
                else
                {
                    var msgName = FirstCharToUpper(name);
                    var packetName = msgName;
                    clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
                }
            }

            var clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
            File.WriteAllText("ClientPacketManager.cs", clientManagerText);
            var serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
            File.WriteAllText("ServerPacketManager.cs", serverManagerText);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
        }
    }
}