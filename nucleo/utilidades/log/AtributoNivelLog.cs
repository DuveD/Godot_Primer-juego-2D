namespace Primerjuego2D.nucleo.utilidades.log;

using System;
using static Primerjuego2D.nucleo.utilidades.log.Logger;

[AttributeUsage(AttributeTargets.Class)]
public class AtributoNivelLog : Attribute
{
    public NivelLog NivelLog { get; }

    public AtributoNivelLog(NivelLog nivelLog)
    {
        this.NivelLog = nivelLog;
    }
}