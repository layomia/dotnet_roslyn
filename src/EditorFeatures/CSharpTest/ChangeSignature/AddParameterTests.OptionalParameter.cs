﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.ChangeSignature;
using Microsoft.CodeAnalysis.Editor.UnitTests.ChangeSignature;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.CodeAnalysis.Test.Utilities.ChangeSignature;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.ChangeSignature
{
    public partial class ChangeSignatureTests : AbstractChangeSignatureTests
    {
        [WpfFact, Trait(Traits.Feature, Traits.Features.ChangeSignature)]
        public async Task AddOptionalParameter_ToEmptySignature_CallsiteOmitted()
        {
            var markup = @"
class C
{
    void M$$()
    {
        M();
    }
}";
            var updatedSignature = new[] {
                new AddedParameterOrExistingIndex(new AddedParameter(null, "int", "a", "", isRequired: false, defaultValue: "1", isCallsiteOmitted: true), "System.Int32") };
            var updatedCode = @"
class C
{
    void M(int a = 1)
    {
        M();
    }
}";

            await TestChangeSignatureViaCommandAsync(LanguageNames.CSharp, markup, updatedSignature: updatedSignature, expectedUpdatedInvocationDocumentCode: updatedCode);
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.ChangeSignature)]
        public async Task AddOptionalParameter_AfterRequiredParameter_CallsiteOmitted()
        {
            var markup = @"
class C
{
    void M$$(int x)
    {
        M(1);
    }
}";
            var updatedSignature = new[] {
                new AddedParameterOrExistingIndex(0),
                new AddedParameterOrExistingIndex(new AddedParameter(null, "int", "a", "", isRequired: false, defaultValue: "1", isCallsiteOmitted: true), "System.Int32") };
            var updatedCode = @"
class C
{
    void M(int x, int a = 1)
    {
        M(1);
    }
}";

            await TestChangeSignatureViaCommandAsync(LanguageNames.CSharp, markup, updatedSignature: updatedSignature, expectedUpdatedInvocationDocumentCode: updatedCode);
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.ChangeSignature)]
        public async Task AddOptionalParameter_BeforeOptionalParameter_CallsiteOmitted()
        {
            var markup = @"
class C
{
    void M$$(int x = 2)
    {
        M()
        M(2);
        M(x: 2);
    }
}";
            var updatedSignature = new[] {
                new AddedParameterOrExistingIndex(new AddedParameter(null, "int", "a", "", isRequired: false, defaultValue: "1", isCallsiteOmitted: true), "System.Int32"),
                new AddedParameterOrExistingIndex(0) };
            var updatedCode = @"
class C
{
    void M(int a = 1, int x = 2)
    {
        M()
        M(x: 2);
        M(x: 2);
    }
}";

            await TestChangeSignatureViaCommandAsync(LanguageNames.CSharp, markup, updatedSignature: updatedSignature, expectedUpdatedInvocationDocumentCode: updatedCode);
        }

        [WpfFact, Trait(Traits.Feature, Traits.Features.ChangeSignature)]
        public async Task AddOptionalParameter_BeforeExpandedParamsArray_CallsiteOmitted()
        {
            var markup = @"
class C
{
    void M$$(params int[] p)
    {
        M();
        M(1);
        M(1, 2);
        M(1, 2, 3);
    }
}";
            var updatedSignature = new[] {
                new AddedParameterOrExistingIndex(new AddedParameter(null, "int", "a", "", isRequired: false, defaultValue: "1", isCallsiteOmitted: true), "System.Int32"),
                new AddedParameterOrExistingIndex(0) };
            var updatedCode = @"
class C
{
    void M(int a = 1, params int[] p)
    {
        M();
        M(p: new int[] { 1 });
        M(p: new int[] { 1, 2 });
        M(p: new int[] { 1, 2, 3 });
    }
}";

            await TestChangeSignatureViaCommandAsync(LanguageNames.CSharp, markup, updatedSignature: updatedSignature, expectedUpdatedInvocationDocumentCode: updatedCode);
        }
    }
}
