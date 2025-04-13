using Microsoft.EntityFrameworkCore;
using MTS.Services.CurriculumAPI.Data;
using MTS.Services.CurriculumAPI.Repository;
using MTS.Services.CurriculumAPI.Repository.IRepository;
using MTS.Services.CurriculumAPI.Services.IService;
using MTS.Services.CurriculumAPI.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<CurriculumDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
});
builder.Services.AddControllers();

builder.Services.AddHttpClient<IUserAPIService, UserAPIService>();

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IWeekRepository, WeekRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IUserAPIService, UserAPIService>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
