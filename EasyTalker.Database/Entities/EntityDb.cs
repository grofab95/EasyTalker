﻿using System;

namespace EasyTalker.Database.Entities;

public abstract class EntityDb
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
}