using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

const string AuthSchema = "cookie";
const string AuthSchema2 = "cookie2";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(AuthSchema)
    .AddCookie(AuthSchema)
    .AddCookie(AuthSchema2);

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("eu passport", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(AuthSchema)
            .AddRequirements(new MyRequirement())
            .RequireClaim("passport_type", "eur");
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, MyRequirementHandler>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// app.Use((ctx, next) =>
// {

//     if (ctx.Request.Path.StartsWithSegments("/login"))
//     {
//         next();
//     }

//     if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthSchema))
//     {
//         ctx.Response.StatusCode = 403;
//         return Task.CompletedTask;
//     }

//     if (!ctx.User.HasClaim("passport_type", "eur"))
//     {
//         ctx.Response.StatusCode = 403;
//         return Task.CompletedTask;
//     }

//     return next();
// });

// [Authorize(Policy = "eu passport")]
app.MapGet("/unsecure", (HttpContext ctx) =>
{
    return new
    {
        Value = ctx.User.FindFirst("usr")?.Value ?? "empty",
        // Passport = ctx.User.FindFirst("passport_type")?.Value ?? ""
    };
});

app.MapGet("/sweden", (HttpContext ctx) =>
{
    // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthSchema))
    // {
    //     ctx.Response.StatusCode = 403;

    //     return "";
    // }

    // if (!ctx.User.HasClaim("passport_type", "eur"))
    // {
    //     ctx.Response.StatusCode = 403;
    //     return "";
    // }

    return "allowed";
}).RequireAuthorization("eu passport");

app.MapGet("/norway", (HttpContext ctx) =>
{
    // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthSchema))
    // {
    //     ctx.Response.StatusCode = 403;

    //     return "";
    // }

    // if (!ctx.User.HasClaim("passport_type", "NOR"))
    // {
    //     ctx.Response.StatusCode = 403;
    //     return "";
    // }

    return "allowed";
}).RequireAuthorization("eu passport");

// [AuthSchema(AuthSchema2)]
// [AuthClaim("passport_type", "eur")]
app.MapGet("/denmark", (HttpContext ctx) =>
{
    // if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthSchema2))
    // {
    //     ctx.Response.StatusCode = 403;

    //     return "";
    // }

    // if (!ctx.User.HasClaim("passport_type", "eur"))
    // {
    //     ctx.Response.StatusCode = 403;
    //     return "";
    // }

    return "allowed";
}).RequireAuthorization("eu passport");



app.MapGet("/login", async (HttpContext ctx) =>
{
    var claims = new List<Claim>();
    claims.Add(new Claim("usr", "rizal"));
    claims.Add(new Claim("passport_type", "eurr"));

    var identity = new ClaimsIdentity(claims, AuthSchema);
    var user = new ClaimsPrincipal(identity);
    await ctx.SignInAsync(AuthSchema, user);
}).AllowAnonymous();

app.Run();

public class MyRequirement : IAuthorizationRequirement { }

public class MyRequirementHandler : AuthorizationHandler<MyRequirement>
{

    public MyRequirementHandler()
    {

    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyRequirement requirement)
    {
        return Task.CompletedTask;
    }
}
