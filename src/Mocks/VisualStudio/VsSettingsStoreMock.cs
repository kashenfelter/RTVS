﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell.Interop;
using NSubstitute;

namespace Microsoft.VisualStudio.Shell.Mocks {
    [ExcludeFromCodeCoverage]
    public static class VsSettingsStoreMock {
        public static IVsWritableSettingsStore Create() {
            return Substitute.For<IVsWritableSettingsStore>();
        }
    }
}
