export const authConfig = {
  auth: {
    clientId: '0a66b744-2412-4f03-82a2-b8bfdcb6ddc2',
    authority: 'https://login.microsoftonline.com/0281b8d2-65ba-4fe1-93b0-3877b20be548',
    redirectUri: '/',
    postLogoutRedirectUri: '/'
  },
  cache: {
      cacheLocation: "sessionStorage"
  }
};

export const resources = {
  api: {
    endpoint: process.env.REACT_APP_API_URL,
    scopes: {
      default: ['api://de3907b0-42c1-48b5-a436-c60cfb4687ef/default'] 
    }
  }
};