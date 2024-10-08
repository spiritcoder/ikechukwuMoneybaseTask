﻿using Moneybase.core;
using Moneybase.core.Interfaces.Services;
using Moneybase.core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKafkaProducer>(sp =>
{
    return new KafkaProducer(EnvironmentVariables.KafkaURL, EnvironmentVariables.KafkaTopic);
});

builder.Services.AddSingleton<IKafkaConsumer>(sp =>
{
    return new KafkaConsumer(EnvironmentVariables.KafkaURL, EnvironmentVariables.KafkaGroupID);
});

builder.Services.AddSingleton<IChatService, ChatService>();

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

