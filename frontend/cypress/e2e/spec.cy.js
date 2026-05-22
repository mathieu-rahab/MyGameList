describe('template spec', () => {
  it('passes', () => {
    cy.visit('http://localhost:5173/')
    cy.get('#page button.btn-login').click();
    cy.get('#Outlet [name="email"]').click();
    cy.get('#Outlet [name="email"]').type('test');
    cy.get('#Outlet [name="password"]').click();
    cy.get('#Outlet [name="password"]').type('test');
    cy.get('#Outlet button.auth-submit').click();
    cy.get('.error-container')
    cy.get('#Outlet [name="email"]').click();
    cy.get('#Outlet [name="email"]').clear();
    cy.get('#Outlet [name="email"]').type('jean@david.co');
    cy.get('#Outlet [name="password"]').click();
    cy.get('#Outlet [name="password"]').clear();
    cy.get('#Outlet [name="password"]').type('YTMRFE9Jj%*qtcYhH&nU');
    cy.get('#Outlet button.auth-submit').click();
    cy.get('#page a[href="/dashboard"] button').click();
  })
})