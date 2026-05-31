describe('AccountLifeCycle', () => {
  it('CRUD de Compte', () => {
    cy.visit('http://localhost:5173/'); // visit site main page
    cy.wait(150);
    cy.get('#page button.btn-login').click(); // go Login
    cy.wait(150); 
    cy.get('#Outlet a').click(); // create account
    cy.wait(150);
    cy.get('[name="pseudo"]').click(); 
    cy.get('[name="pseudo"]').type('azerty');
    cy.get('[name="email"]').click();
    cy.get('[name="email"]').type('azerty@azerty.az');
    cy.get('[name="password"]').click();
    cy.get('[name="password"]').type('Azerty456');
    cy.get('[name="passwordConfirm"]').click();
    cy.get('[name="passwordConfirm"]').type('Azerty456');
    cy.get('#Outlet button.auth-submit').click();
    cy.wait(3500);
    cy.get('#Outlet [name="password"]').click(); // Log in
    cy.get('#Outlet [name="password"]').type('Azerty456');
    cy.get('#Outlet button.auth-submit').click();
    cy.get('#page a[href="/dashboard"] button').click(); // config account
    cy.wait(150);
    cy.get('#Outlet a').click();
    cy.get('#Outlet input[placeholder="Nouveau Pseudo"]').click(); // change pseudo
    cy.get('#Outlet input[placeholder="Nouveau Pseudo"]').clear();
    cy.get('#Outlet input[placeholder="Nouveau Pseudo"]').type('David');
    cy.get('#Outlet div.changing_pseudo input[type="button"]').click();
    cy.wait(150);
    cy.get('#page a[href="/dashboard"] button').click();
    cy.wait(150);
    cy.get('#Outlet a').click();
    cy.wait(150);
    cy.get('#Outlet input[placeholder="Nouveau SteamID"]').click();
    cy.get('#Outlet input[placeholder="Nouveau SteamID"]').type('76561198296985493');
    cy.get('#Outlet div.changing_steamid input[type="button"]').click();
    cy.wait(150);
    cy.get('#page a[href="/dashboard"] button').click();
    cy.wait(500);
    cy.get('#page div.foot-in div.theme-toggle div:nth-child(2)').click();
    cy.get('#page button.btn-login').click();
    cy.wait(150);
    cy.get('#page div.hright div.theme-toggle div:nth-child(2)').click();
    cy.get('#Outlet [name="email"]').click();
    cy.get('#Outlet [name="email"]').type('azerty@azerty.az');
    cy.get('#Outlet [name="password"]').type('Azerty456');
    cy.get('#Outlet button.auth-submit').click();
    cy.intercept('GET', '**/api/User/*').as('getUserData');
    cy.wait('@getUserData')
    cy.get('#page div.av').click();
    cy.wait(500);
    cy.intercept('DELETE', '**/api/User/*').as('deleteAccount');
    cy.get('#Outlet input[value="Delete your account"]').click();
    cy.wait('@deleteAccount').then((interception) => {
        expect(interception.response.statusCode).to.eq(200); 
    });
    cy.reload();
    cy.get('#page button.btn-login').click(); // go Login
    cy.wait(150);
    cy.get('#Outlet [name="email"]').click();
    cy.get('#Outlet [name="email"]').type('azerty@azerty.az');
    cy.get('#Outlet [name="password"]').type('Azerty456');
    cy.get('#Outlet button.auth-submit').click();
    cy.get('#page #home-button').click();
  })
})