using System;
using System.Collections.Generic;

namespace demo2.Models;

public partial class Servicephoto
{
    public int Id { get; set; }

    public int Serviceid { get; set; }

    public string Photopath { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
