using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using HostingEnvironments = Microsoft.Extensions.Hosting.Environments;

namespace DivergentFlow.Api.Tests.TestDoubles;

public sealed class FakeWebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "DivergentFlow.Api.Tests";

    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();

    public string WebRootPath { get; set; } = string.Empty;

    public string EnvironmentName { get; set; } = HostingEnvironments.Production;

    public string ContentRootPath { get; set; } = string.Empty;

    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
