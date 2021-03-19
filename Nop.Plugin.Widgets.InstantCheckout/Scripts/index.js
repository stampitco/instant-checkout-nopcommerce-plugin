const GET_CHECKOUT_URL_STARTED = 'stamp-ic-checkout:get-checkout-url-started';
const GET_CHECKOUT_URL_SUCCESS = 'stamp-ic-checkout:get-checkout-url-success';
const GET_CHECKOUT_URL_ERROR = 'stamp-ic-checkout:get-checkout-url-error';

const CHECKOUT_WINDOW_OPENED = 'stamp-ic-checkout:checkout-window-opened';
const CHECKOUT_WINDOW_CLOSED = 'stamp-ic-checkout:checkout-window-closed';
const CHECKOUT_WINDOW_FOCUSED = 'stamp-ic-checkout:checkout-window-focused';

const Checkout = function Checkout(params) {

  const {
    $button,
    api,
    debug,
    mediator,
  } = params;

  this.$button = $button;
  this.api = api;
  this.debug = debug;
  this.mediator = mediator;

  this.init();
}

Checkout.prototype.init = function init() {
  this.mediator.subscribe(CHECKOUT_WINDOW_CLOSED, this.onCheckoutWindowClosed.bind(this));
  this.bindEvents();
};

Checkout.prototype.bindEvents = function bindEvents() {
  this.$button.click(this.onCheckoutButtonClick.bind(this));
};

Checkout.prototype.onCheckoutWindowClosed = function onCheckoutWindowClosed() {
  this.disableButtonLoading();
};

Checkout.prototype.onCheckoutButtonClick = async function onCheckoutButtonClick(event) {

  event.preventDefault();

  this.enableButtonLoading();

  const params = this.getCheckoutParams();

  if (!params) {
    return;
  }

  this.$button.trigger(GET_CHECKOUT_URL_STARTED, [params]);
  this.mediator.publish(GET_CHECKOUT_URL_STARTED, params);

  try {
    const checkoutUrl = `${params.ic_base_url}?productId=${params.product_id}&sku=&quantity=${params.qty}&appKey=${params.app_id}`;
    const result = { checkout_url: checkoutUrl };
    this.$button.trigger(GET_CHECKOUT_URL_SUCCESS, [params, result]);
    this.mediator.publish(GET_CHECKOUT_URL_SUCCESS, params, result);
  } catch (error) {
    this.$button.trigger(GET_CHECKOUT_URL_ERROR, [params, error]);
    this.disableButtonLoading();
    if (this.debug) {
      console.error('Instant Checkout: Failed to get the checkout url from the backend' + error);
    }
  }
}

Checkout.prototype.disableButtonLoading = function disableButtonLoading() {
  this.$button.attr({ disabled: false }).removeClass('stamp-ic-checkout-loading');
};

Checkout.prototype.enableButtonLoading = function enableButtonLoading() {
  this.$button.attr({ disabled: true }).addClass('stamp-ic-checkout-loading');
};

Checkout.prototype.getCheckoutParams = function getCheckoutParams() {

  const $cartForm = this.$button.parents('#stamp-ic-checkout-section');

  if ($cartForm.length === 0) {
    if (this.debug) {
      console.error('Instant Checkout: Cart form html element missing');
    }
    return false;
  }

  const qty = parseInt($('input.qty-input').val());

  if (!qty) {
    if (this.debug) {
      console.error('Instant Checkout: Product Qty is empty');
    }
    this.$button.trigger(
      GET_CHECKOUT_URL_ERROR,
      [
        {
          errors: [
            {
              param: 'qty',
              message: 'stamp_ic_wc_qty_param_empty'
            }
          ]
        }
      ]
    );
    return false;
  }

  const ic_base_url = $cartForm.find('#stamp-ic-base-url').val();
  const app_id = $cartForm.find('#stamp-ic-merchant-id').val();


  const data = {
    app_id,
    ic_base_url,
    product_id: this.$button.data('product-id'),
    qty,
  };

  //if ($cartForm.hasClass('variations_form')) {

  //  const variation_id = parseInt($cartForm.find('input[name="variation_id"]').val());

  //  if (!variation_id) {
  //    if (this.debug) {
  //      console.error('Instant Checkout: Product Variation is empty');
  //    }
  //    this.$button.trigger(
  //      GET_CHECKOUT_URL_ERROR,
  //      [
  //        {
  //          errors: [
  //            {
  //              param: 'variation_id',
  //              message: 'stamp_ic_wc_variation_id_param_empty'
  //            }
  //          ]
  //        }
  //      ]
  //    );
  //    return false;
  //  }

  //  data['variation_id'] = variation_id;
  //}

  return data;
};

Checkout.prototype.isOnSingleProductPage = function isOnSingleProductPage() {
  const $body = $('body');
  return $body.hasClass('single-product') && $body.hasClass('woocommerce')
};


const CheckoutWindow = function CheckoutWindow({ $invoker, mediator }) {
  this.mediator = mediator;
  this.$invoker = $invoker;
  this.popup = null;
  this.monitorInterval = null;
  this.init();
}

CheckoutWindow.prototype.init = function init() {
  this.mediator.subscribe(GET_CHECKOUT_URL_STARTED, this.onGetCheckoutUrlStarted.bind(this));
  this.mediator.subscribe(GET_CHECKOUT_URL_ERROR, this.onGetCheckoutUrlError.bind(this));
  this.mediator.subscribe(GET_CHECKOUT_URL_SUCCESS, this.onGetCheckoutUrlSuccess.bind(this));
  this.mediator.subscribe(CHECKOUT_WINDOW_FOCUSED, this.onCheckoutWindowFocused.bind(this));
};

CheckoutWindow.prototype.onGetCheckoutUrlStarted = function onGetCheckoutUrlStarted() {
  this.open({ url: '' });
};

CheckoutWindow.prototype.onGetCheckoutUrlError = function onGetCheckoutUrlError() {
  this.close();
};

CheckoutWindow.prototype.onCheckoutWindowFocused = function onCheckoutWindowFocused() {
  if (this.popup) {
    this.popup.focus();
  }
};

CheckoutWindow.prototype.onGetCheckoutUrlSuccess = function onGetCheckoutUrlSuccess(params, result) {
  this.setUrl(result.checkout_url);
};

CheckoutWindow.prototype.open = function open({ url }) {
  if (!this.popup) {
    const params = 'scrollbars=no,resizable=no,status=no,location=no,toolbar=no,menubar=no,width=400,height=900';
    this.popup = window.open(url || '', '_blank', params);
    $(document).trigger(CHECKOUT_WINDOW_OPENED);
    this.monitorInterval = window.setInterval(this.monitor.bind(this), 600);
  }
};

CheckoutWindow.prototype.monitor = function monitor() {
  if (this.monitorInterval) {
    try {
      if (this.popup == null || this.popup.closed) {
        window.clearInterval(this.monitorInterval);
        this.close();
      }
    }
    catch (error) {
      console.error(error);
    }
  }
};

CheckoutWindow.prototype.setUrl = function setUrl(url) {
  if (this.popup) {
    this.popup.location.href = url;
  }
};

CheckoutWindow.prototype.close = function close() {
  if (this.popup) {
    this.popup.close();
    $(document).trigger(CHECKOUT_WINDOW_CLOSED);
    this.mediator.publish(CHECKOUT_WINDOW_CLOSED);
    this.popup = null;
  }
};

const CheckoutOverlay = function CheckoutOverlay({ logo, linkText, overlayText, mediator }) {
  this.logo = logo;
  this.linkText = linkText;
  this.overlayText = overlayText;
  this.mediator = mediator;
  this.$element = null;
  this.init();
}

CheckoutOverlay.prototype.init = function init() {
  this.mediator.subscribe(GET_CHECKOUT_URL_STARTED, this.onGetCheckoutUrlStarted.bind(this));
  this.mediator.subscribe(GET_CHECKOUT_URL_ERROR, this.onGetCheckoutUrlError.bind(this));
  this.mediator.subscribe(CHECKOUT_WINDOW_CLOSED, this.onCheckoutWindowClosed.bind(this));
  this.bindEvents();
};

CheckoutOverlay.prototype.bindEvents = function bindEvents() {
  if (this.$element) {
    this.$element.find('#stamp-ic-wc-overlay-link').click(this.onOverlayLinkClick.bind(this));
  }
};

CheckoutOverlay.prototype.onGetCheckoutUrlStarted = function onGetCheckoutUrlStarted() {
  this.open();
}

CheckoutOverlay.prototype.onGetCheckoutUrlError = function onGetCheckoutUrlError() {
  this.close();
}

CheckoutOverlay.prototype.onCheckoutWindowClosed = function onCheckoutWindowClosed() {
  this.close();
}

CheckoutOverlay.prototype.open = function open() {
  if (!this.$element) {
    this.$element = $(this.getHtml());
    $('body').append(this.$element);
    this.bindEvents();
  }
  this.$element.addClass('stamp-ic-wc-overlay-active')
};

CheckoutOverlay.prototype.onOverlayLinkClick = function onOverlayLinkClick(event) {
  event.preventDefault();
  this.mediator.publish(CHECKOUT_WINDOW_FOCUSED);
};

CheckoutOverlay.prototype.isOpened = function isOpened() {
  return this.$element && this.$element.hasClass('stamp-ic-wc-overlay-active');
};

CheckoutOverlay.prototype.close = function close() {
  if (this.$element) {
    this.$element.removeClass('stamp-ic-wc-overlay-active')
  }
};

CheckoutOverlay.prototype.remove = function remove() {
  if (this.$element) {
    this.$element.remove();
  }
};

CheckoutOverlay.prototype.getHtml = function getHtml() {
  return `
        <div id="stamp-ic-wc-overlay">
            <div id="stamp-ic-wc-overlay-modal">
                <img src="${this.logo}" alt="${this.linkText}" id="stamp-ic-wc-overlay-logo">
                <p id="stamp-ic-wc-overlay-text">${this.overlayText}</p>
                <a href="#" id="stamp-ic-wc-overlay-link">${this.linkText}</a>
            </div>
        </div>
    `
};


const Mediator = function Mediator({ topics }) {
  this.topics = topics || {};
}

Mediator.prototype.subscribe = function subscribe(topic, fn) {

  if (!this.topics.hasOwnProperty(topic)) {
    this.topics[topic] = [];
  }

  this.topics[topic].push({ context: this, callback: fn });

  return this;
};

Mediator.prototype.publish = function publish(topic) {

  if (!this.topics.hasOwnProperty(topic)) {
    return false;
  }

  let args = Array.prototype.slice.call(arguments, 1);

  for (let i = 0, l = this.topics[topic].length; i < l; i++) {
    let subscription = this.topics[topic][i];
    subscription.callback.apply(subscription.context, args);
  }
  return this;
};

$(function () {

  const $checkoutButton = $('.stamp-ic-checkout-button');
  const stampIcCheckout = {
    overlay: {
      linkText: "Click here",
      logo: "/Plugins/Widgets.InstantCheckout/Content/instantcheckout/instant_checkout-logo.png",
      overlayText: "No longer see the Instant Checkout window"
    }
  }


  if ($checkoutButton.length > 0 && stampIcCheckout) {

    const api = {};
    const mediator = new Mediator({});

    const checkoutWindow = new CheckoutWindow({ mediator });
    const checkoutOverlay = new CheckoutOverlay({ ...stampIcCheckout.overlay || {}, mediator });

    $checkoutButton.each(function initCheckout() {
      new Checkout({
        $button: $(this),
        api,
        mediator,
        debug: parseInt(stampIcCheckout.debug) !== 0,
      });
    });
  }
});