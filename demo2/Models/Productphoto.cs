﻿using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class Productphoto
{
    public int Id { get; set; }

    public int Productid { get; set; }

    public string Photopath { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
