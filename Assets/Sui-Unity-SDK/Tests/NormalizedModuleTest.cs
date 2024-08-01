//
//  NormalizedModuleTest.cs
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

using System.Collections;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using Sui.Rpc;
using Sui.Rpc.Models;
using System.Linq;
using System.Collections.Generic;
using Sui.Types;
using Sui.Accounts;

namespace Sui.Tests
{
    public class NormalizedModuleTest
    {
        TestToolbox Toolbox;
        AccountAddress DefaultPackage;
        SuiStructTag BalanceFunctionTag;
        SuiStructTag CoinStruct;

        readonly string DefaultModule = "coin";
        readonly string DefaultFunction = "balance";

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            this.Toolbox = new TestToolbox();
            yield return this.Toolbox.Setup();

            this.BalanceFunctionTag = new SuiStructTag("0x2::coin::balance");
            this.CoinStruct = new SuiStructTag("0x2::coin::Coin");
            this.DefaultPackage = AccountAddress.FromHex("0x2");
        }

        [UnityTest]
        public IEnumerator MoveFunctionArgTypeFetchTest()
        {
            Task<RpcResult<IEnumerable<MoveFunctionArgType>>> arg_types_task = this.Toolbox.Client.GetMoveFunctionArgTypesAsync(this.BalanceFunctionTag);
            yield return new WaitUntil(() => arg_types_task.IsCompleted);

            Assert.IsTrue(arg_types_task.Result.Result.SequenceEqual(new MoveFunctionArgType[]
            {
                new MoveFunctionArgType(ArgumentType.Object, ObjectValueType.ByImmutableReference)
            }));
        }

        [UnityTest]
        public IEnumerator MoveFunctionFetchByPackageTest()
        {
            Task<RpcResult<System.Collections.Generic.Dictionary<string, SuiMoveNormalizedModule>>> package_task = this.Toolbox.Client.GetNormalizedMoveModulesByPackageAsync(this.DefaultPackage);
            yield return new WaitUntil(() => package_task.IsCompleted);

            if (package_task.Result.Error != null)
                Assert.Fail(package_task.Result.Error.Message);

            Assert.IsTrue(package_task.Result.Result.ContainsKey(this.DefaultModule));
        }

        [UnityTest]
        public IEnumerator NormalizedMoveModuleFetchTest()
        {
            Task<RpcResult<SuiMoveNormalizedModule>> normalized_move_module_task = this.Toolbox.Client.GetNormalizedMoveModuleAsync(this.DefaultPackage, this.DefaultModule);
            yield return new WaitUntil(() => normalized_move_module_task.IsCompleted);

            Assert.IsTrue(normalized_move_module_task.Result.Result.ExposedFunctions.ContainsKey(this.DefaultFunction));
        }

        [UnityTest]
        public IEnumerator NormalizedMoveFunctionFetchTest()
        {
            Task<RpcResult<NormalizedMoveFunctionResponse>> normalized_move_function_task = this.Toolbox.Client.GetNormalizedMoveFunctionAsync
            (
                this.BalanceFunctionTag
            );
            yield return new WaitUntil(() => normalized_move_function_task.IsCompleted);

            Assert.IsFalse(normalized_move_function_task.Result.Result.IsEntry);
        }

        [UnityTest]
        public IEnumerator NormalizedMoveStructFetchTest()
        {
            Task<RpcResult<SuiMoveNormalizedStruct>> normalized_move_struct_test = this.Toolbox.Client.GetNormalizedMoveStructAsync
            (
                this.CoinStruct
            );
            yield return new WaitUntil(() => normalized_move_struct_test.IsCompleted);

            Assert.Greater(normalized_move_struct_test.Result.Result.Fields.Count(), 1);
        }
    }
}
