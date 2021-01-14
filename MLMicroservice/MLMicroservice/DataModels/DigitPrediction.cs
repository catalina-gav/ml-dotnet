using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLMicroservice.MLModels
{
    public class DigitPrediction 
    {
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [ColumnName("PredictedLabel")]
        public Single Prediction { get; set; }
        public float[] Score { get; set; }
    }
}
