﻿using System;

namespace B1Task1;

public interface IGenerator
{
    public string GenerateRussianString();
    public string GenerateEnglishString();
    public DateOnly GenerateDate();
    public int GenerateInt();
    public double GenerateDouble();
}