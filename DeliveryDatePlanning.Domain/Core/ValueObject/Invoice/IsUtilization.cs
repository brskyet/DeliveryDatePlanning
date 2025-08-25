using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice
{
    public class IsUtilization : SimpleValueObject<bool>
    {
        private IsUtilization(bool value) : base(value)
        {
        }

        public static Result<IsUtilization> Create(bool value)
        {
            return new IsUtilization(value);
        }
    }
}
