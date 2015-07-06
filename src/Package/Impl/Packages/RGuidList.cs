﻿using System;

namespace Microsoft.VisualStudio.R.Package
{
    static class RGuidList
    {
        public const string EditorFactoryGuidString = "EE606CC0-077A-4FDE-91C3-24EC012C8389";
        public const string LanguageServiceGuidString = "29C0D8E0-C01C-412B-BEE8-7A7A253A31E6";
        public const string PackageGuidString = "6D7C5336-C0CA-4857-A7E7-2E964EA836BF";
        public const string CmdSetGuidString = "AD87578C-B324-44DC-A12A-B01A6ED5C6E3";

        public static readonly Guid EditorFactoryGuid = new Guid(EditorFactoryGuidString);
        public static readonly Guid LanguageServiceGuid = new Guid(LanguageServiceGuidString);
        public static readonly Guid PackageGuid = new Guid(PackageGuidString);
        public static readonly Guid CmdSetGuid = new Guid(CmdSetGuidString);
    };
}