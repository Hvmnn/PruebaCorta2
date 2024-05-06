using chairs_dotnet7_api;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var chairs = app.MapGroup("api/chair");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS
chairs.MapGet("/", GetChairs);
chairs.MapGet("/{name}", GetChairByName);
chairs.MapPost("/", CreateChair);
chairs.MapPost("/{purchase}", PurchaseChair);
chairs.MapPut("/{id}", UpdateChair);
chairs.MapPut("/{id}/stock", IncStockChair);
chairs.MapDelete("/{id}", DeleteChair);

app.Run();

//TODO: ENDPOINTS SOLICITADOS
static async Task<IResult> GetChairs(DataContext db)
{
    return TypedResults.Ok(await db.Chairs.ToArrayAsync());
}

static async Task<IResult> CreateChair (DataContext db, Chair chair)
{
    var chairFound = await db.Chairs.FindAsync(chair.Nombre);

    if (chairFound is null){
        return TypedResults.BadRequest("La silla ya existe");
    }

    await db.AddAsync(chair);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/api/chair/{chair.Id}", chair);
}

static async Task<IResult> GetChairByName (DataContext db, string nombre)
{
    var chairFound = await db.Chairs.FindAsync(nombre);

    if(chairFound is null){
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(chairFound);
}

static IResult PurchaseChair (DataContext db)
{
    return TypedResults.Ok();
}

static async Task<IResult> UpdateChair (DataContext db, int id, Chair inputChair)
{
    var chairFound = await db.Chairs.FindAsync(id);

    if (chairFound is null){
        return TypedResults.NotFound();
    }
    chairFound.Nombre = inputChair.Nombre;
    chairFound.Tipo = inputChair.Tipo;
    chairFound.Material = inputChair.Material;
    chairFound.Color = inputChair.Color;
    chairFound.Altura = inputChair.Altura;
    chairFound.Anchura = inputChair.Anchura;
    chairFound.Profundidad = inputChair.Profundidad;
    chairFound.Precio = inputChair.Precio;

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
    
}

static IResult IncStockChair(DataContext db)
{
    return TypedResults.Ok();
}

static async Task<IResult> DeleteChair (DataContext db, int id)
{
    if(await db.Chairs.FindAsync(id) is Chair chair)
    {
        db.Chairs.Remove(chair);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    return TypedResults.Ok();
}


