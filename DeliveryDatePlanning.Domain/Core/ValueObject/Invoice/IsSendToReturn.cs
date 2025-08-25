using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice
{
    public class IsSendToReturn : SimpleValueObject<bool>
    {
        private IsSendToReturn(bool value) : base(value)
        {
        }

        public static Result<IsSendToReturn> Create(bool value)
        {
            return new IsSendToReturn(value);
        }
    }
}
