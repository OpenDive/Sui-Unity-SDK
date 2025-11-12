//
//  Command.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Utilities;

namespace Sui.Transactions
{
    /// <summary>
    /// <para>A Command is an instruction to be executed on-chain. A Command can be:</para>
    /// <para>MakeMove, MergeCoin, MoveCall, Publish, SplitCOins, TransferObject, Upgrade</para>
    /// </summary>
    [JsonConverter(typeof(CommandConverter))]
    public class Command : ReturnBase, ISerializable
    {
        /// <summary>
        /// The kind of command.
        /// </summary>
        public CommandKind Kind { get; private set; }

        /// <summary>
        /// The internal representation of a command.
        /// </summary>
        private ICommand command;

        /// <summary>
        /// The public representation of a command.
        /// Uses a safe set method to change the command's kind accordingly.
        /// </summary>
        public ICommand Function
        {
            get => this.command;
            set
            {
                if (value.GetType() == typeof(MoveCall))
                    this.Kind = CommandKind.MoveCall;
                else if (value.GetType() == typeof(TransferObjects))
                    this.Kind = CommandKind.TransferObjects;
                else if (value.GetType() == typeof(SplitCoins))
                    this.Kind = CommandKind.SplitCoins;
                else if (value.GetType() == typeof(MergeCoins))
                    this.Kind = CommandKind.MergeCoins;
                else if (value.GetType() == typeof(Publish))
                    this.Kind = CommandKind.Publish;
                else if (value.GetType() == typeof(MakeMoveVec))
                    this.Kind = CommandKind.MakeMoveVec;
                else if (value.GetType() == typeof(Upgrade))
                    this.Kind = CommandKind.Upgrade;
                else
                {
                    this.SetError<SuiError>("Unable to set Command.");
                    return;
                }

                this.command = value;
            }
        }

        public Command(CommandKind kind, ICommand command)
        {
            this.Kind = kind;
            this.command = command;
        }

        public void Serialize(Serialization serializer)
        {
            switch(this.Kind)
            {
                case CommandKind.MoveCall:
                    serializer.SerializeU8(0);
                    break;
                case CommandKind.TransferObjects:
                    serializer.SerializeU8(1);
                    break;
                case CommandKind.SplitCoins:
                    serializer.SerializeU8(2);
                    break;
                case CommandKind.MergeCoins:
                    serializer.SerializeU8(3);
                    break;
                case CommandKind.Publish:
                    serializer.SerializeU8(4);
                    break;
                case CommandKind.MakeMoveVec:
                    serializer.SerializeU8(5);
                    break;
                case CommandKind.Upgrade:
                    serializer.SerializeU8(6);
                    break;
            }

            serializer.Serialize(this.Function);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte value = deserializer.DeserializeU8().Value;

            switch (value)
            {
                case 0:
                    return new Command
                    (
                        CommandKind.MoveCall,
                        (MoveCall)MoveCall.Deserialize(deserializer)
                    );
                case 1:
                    return new Command
                    (
                        CommandKind.TransferObjects,
                        (TransferObjects)TransferObjects.Deserialize(deserializer)
                    );
                case 2:
                    return new Command
                    (
                        CommandKind.SplitCoins,
                        (SplitCoins)SplitCoins.Deserialize(deserializer)
                    );
                case 3:
                    return new Command
                    (
                        CommandKind.MergeCoins,
                        (MergeCoins)MergeCoins.Deserialize(deserializer)
                    );
                case 4:
                    return new Command
                    (
                        CommandKind.Publish,
                        (Publish)Publish.Deserialize(deserializer)
                    );
                case 5:
                    return new Command
                    (
                        CommandKind.MakeMoveVec,
                        (MakeMoveVec)MakeMoveVec.Deserialize(deserializer)
                    );
                case 6:
                    return new Command
                    (
                        CommandKind.Upgrade,
                        (Upgrade)Upgrade.Deserialize(deserializer)
                    );
                default:
                    return new SuiError(0, "Unable to deserialize Command.", null);
            }
        }
    }
}