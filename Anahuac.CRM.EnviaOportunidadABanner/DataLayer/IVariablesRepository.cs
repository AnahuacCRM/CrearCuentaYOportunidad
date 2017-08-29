using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anahuac.CRM.EnviaOportunidadABanner.DataLayer
{
    interface IVariablesRepository
    {
        string ObtenerVariableSistema(string EntityLogicalName, string Variable);

        string ObtenerPeriodo(Guid periodoId);
        string ObtenerVPDI(Guid VPDId);
        string ObtenerPrograma(Guid programaId);

     
    }
}
