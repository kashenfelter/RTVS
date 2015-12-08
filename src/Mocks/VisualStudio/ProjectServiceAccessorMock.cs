﻿using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.ProjectSystem;

namespace Microsoft.VisualStudio.Shell.Mocks {
    [ExcludeFromCodeCoverage]
    [Export(typeof(IProjectServiceAccessor))]
    public sealed class ProjectServiceAccessorMock : IProjectServiceAccessor {
        private Lazy<ProjectService> _projectServiceMock = new Lazy<ProjectService>(() => new ProjectServiceMock());
        public ProjectService GetProjectService(ProjectServiceThreadingModel threadingModel = ProjectServiceThreadingModel.Multithreaded) {
            return _projectServiceMock.Value;
        }
    }
}
