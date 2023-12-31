﻿using System;

namespace B1Task1.Data;

public interface IDataGenerator
{
    public string GenerateRussianString();
    public string GenerateEnglishString();
    public DateOnly GenerateDate();
    public int GenerateInt();
    public double GenerateDouble();
}