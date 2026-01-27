using FluentValidation;
using OrderManagement.API.DTOs;

namespace OrderManagement.API.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    //burada bazı validasyon kurallarını tanımlıyoruz
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Ürün adı boş olamaz")
            .MaximumLength(200).WithMessage("Ürün adı en fazla 200 karakter olabilir")
            .MinimumLength(3).WithMessage("Ürün adı en az 3 karakter olmalıdır");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır")
            .LessThanOrEqualTo(999999999999999.99m).WithMessage("Fiyat çok yüksek");
    }
}