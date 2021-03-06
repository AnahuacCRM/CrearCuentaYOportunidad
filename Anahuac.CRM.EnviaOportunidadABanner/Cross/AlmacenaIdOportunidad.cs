﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anahuac.CRM.EnviaOportunidadABanner.Cross
{
    public class AlmacenaIdOportunidad : BusinessTypeBase
    {
        //[Required(ErrorMessage = "El atributo {0} es requerido.")]
        [MaxLength(36, ErrorMessage = "La longitud maxima para el atributo {0} es de {1} caracteres")]
        public string Id_Oportunidad { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        [MaxLength(36, ErrorMessage = "La longitud maxima para el atributo {0} es de {1} caracteres")]
        public string Id_Cuenta { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        [MaxLength(9, ErrorMessage = "La longitud maxima para el atributo {0} es de {1} caracteres")]
        public string Id_Banner { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        [MaxLength(6, ErrorMessage = "La longitud maxima para el atributo {0} es de {1} caracteres")]
        public string Periodo { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        [MaxLength(4, ErrorMessage = "La longitud maxima para el atributo {0} es de {1} caracteres")]
        public string VPD { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        [MaxLength(12, ErrorMessage = "La longitud maxima para el atributo {0} es de {1} caracteres")]
        public string Programa { get; set; }

        public int Numero_Solicitud { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string Correo_Reclutador { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string Nombre_Reclutador { get; set; }

        //[Required(ErrorMessage = "El atributo {0} es requerido.")]
        public string Escuela { get; set; }

        [Required(ErrorMessage = "El atributo {0} es requerido.")]
        public DateTime Fecha_Creacion { get; set; }
        
        public string Origen { get; set; }

        //Ticket 4543
        public string Campus { get; set; }
        public string Nivel { get; set; }

        



    }
}
