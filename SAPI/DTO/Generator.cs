﻿using MongoDB.Bson;
using System;

namespace DTO
{
    public class Generator
    {
        public static string Id()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public static long Tick()
        {
            return DateTime.UtcNow.Ticks;
        }
    }
}