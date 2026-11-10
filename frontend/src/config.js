// Site-wide settings. Change the name or tagline here and every page follows.
export const siteName = 'Words of Life';
export const tagline = 'Light for every life';

// The Vite development server proxies /api to the ASP.NET Core backend.
// Set VITE_API_BASE_URL only when the API is hosted on a separate address.
export const apiBaseUrl = import.meta.env.VITE_API_BASE_URL || '';

// Donation details are shown on the Donate page only once they are filled in.
// Leave a value empty until the ministry has confirmed it.
export const donation = {
  mpesaPaybill: '',
  mpesaAccount: '',
  bankName: '',
  bankAccountName: '',
  bankAccountNumber: '',
  contactEmail: ''
};
