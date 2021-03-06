﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Sining.Tools;

namespace Sining.ProtoBufTool
{
    internal class OpcodeInfo
    {
        public string Name;
        public int Opcode;

        public OpcodeInfo(string name, int opcode)
        {
            Name = name;
            Opcode = opcode;
        }
    }
    
    internal static class Program
    {
        private const string OuterMessageName = "OuterMessage.proto";
        private const string OuterOpcodeName = "OuterOpcode";
        private const string OuterOpcodeCsName = OuterOpcodeName + ".cs";
        private const string ProtoBufPath = "../../../../../ProtoBuf/";
        private const string ServerPath = "../Server/Model/Base/Network/Message/";
        
        private static readonly List<OpcodeInfo> Opcodes = new List<OpcodeInfo>();
        
        private static readonly char[] SplitChars = { ' ', '\t' };

        private static void Main(string[] args)
        {
            string protoToolName;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                protoToolName = "protoc.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                protoToolName = "protoc";
            }
            else
            {
                Console.WriteLine("ProtoBufTool不支持当前系统!");

                return;
            }

            ProcessHelper.Run(
                Path.Combine(ProtoBufPath, protoToolName),
                $"--proto_path=./ {OuterMessageName} --csharp_out={ServerPath}",
                ProtoBufPath, true);

            OuterOpcode();
            
            Console.WriteLine("proto2cs succeed!");
        }

        private static void OuterOpcode()
        {
            var startOpcode = 100;
            
            using var protoFile = new StreamReader(Path.Combine(ProtoBufPath, OuterMessageName));

            var file = new StringBuilder();

            file.Append("using Sining.Network;\n\n");
            file.Append("namespace Sining.Message\n");
            file.Append("{\n");

            for (;;)
            {
                var line = protoFile.ReadLine();

                if (line == null) break;

                if (string.IsNullOrWhiteSpace(line)) continue;
                
                if (line.StartsWith("//"))
                {
                    file.Append($"\t{line}\n");
                    continue;
                }

                if (!line.StartsWith("message")) continue;
                
                var className = line.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries)[1];
                var interfaceName = line.Split(new[] {"//"}, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                file.Append($"\t[Message({OuterOpcodeName}.{className})]\n");
                file.Append($"\tpublic partial class {className} ");
                file.Append($": {interfaceName} ");
                file.Append("{}\n\n");
                Opcodes.Add(new OpcodeInfo(className, ++startOpcode));
            }

            file.Append("}\n");

            GenerateOpcode(file);

            using var sw = new StreamWriter($"../../../../{ServerPath}{OuterOpcodeCsName}");
            
            sw.Write(file.ToString());
        }

        private static void GenerateOpcode(StringBuilder file)
        {
            file.AppendLine("namespace Sining.Message");
            file.AppendLine("{");
            file.AppendLine($"\tpublic static partial class {OuterOpcodeName}");
            file.AppendLine("\t{");
            foreach (var info in Opcodes)
            {
                file.AppendLine($"\t\t public const ushort {info.Name} = {info.Opcode};");
            }
            Opcodes.Clear();
            file.AppendLine("\t}");
            file.AppendLine("}");
        }
    }
}