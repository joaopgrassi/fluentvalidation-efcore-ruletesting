# Unit testing Fluent Validation rules against your EF Core model

This is the acompaning repo for my [blog post](https://blog.joaograssi.com/unit-testing-fluent-validation-rules-against-your-ef-core-model/) about making sure your Fluent Validators are "in-sync" with your EF Core entity configuration.  
  
The idea is that with the help of unit tests, we can find out if our validator is out of sync. Given this example Entity + EF Core configuration:

**Customer entity**

```
public class Customer
{
    public int Id { get; set; }

    public string Surname { get; set; }

    public string Forename { get; set; }

    public string Address { get; set; }
}
```

**IEntityTypeConfiguration<Customer><Customer>**

```
public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
    	builder.HasKey(c => c.Id);
    	builder.Property(c => c.Surname).IsRequired().HasMaxLength(255);
    	builder.Property(c => c.Forename).IsRequired().HasMaxLength(255);
    	builder.Property(c => c.Address).HasMaxLength(250);
    }
}
```

And this Fluent Validator:

```
public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
    	RuleFor(x => x.Surname)
    		.NotEmpty()
    		.MaximumLength(255)
    		.WithMessage("Please specify a last name");
    
    	RuleFor(x => x.Forename)
    		.NotEmpty()
    		.MaximumLength(255)
    		.WithMessage("Please specify a first name");
    
    	RuleFor(x => x.Address).Length(20, 250);
    }
}
```

See how our validator is tighly coupled with our EF model configuration? For example the *length of 255* is present on both sides. If we change the EF model, say due to a new requirement, nothing in our code base will "alert" us that we need to also modify the validator.

This repo contain a simple ASP.NET Core API which uses EF Core + Fluent Validation. The interesting part (where the solution is located) is the `Shop.API.Tests` project.

# Requirements

- .NET Core 7.0



