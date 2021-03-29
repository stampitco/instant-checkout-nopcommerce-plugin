# Instant Checkout for NopCommerce

### Plugin install:

After you activate the widget you need to configure the settings that the widget requires:

- MerchantId provided by the Instant Checkout team.

You can access them from Admin:

```
/Admin/WidgetsInstantCheckout/Configure
```

### Button appearance in a theme:

The button appears in two pages:

- In the product details page - the `.add-to-cart-button` class that is used by default to output the button html
- In the cart page - the `.checkout-button` class that is used by default to output the button html

You can add custom html attributes to the button changing the styles in `Content\instantcheckout\instant-checkout.css`:
