using AssetManager.Infrastructure.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Validators
{
    public class AttributeModelValidator : AbstractValidator<AttributeModel>
    {
        public AttributeModelValidator()
        {
        }
    }
}