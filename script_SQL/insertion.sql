INSERT INTO utilisateur(nom,prenom,ville,pays,email,motDePasse)
	VALUES
	('Dupont', 'Jean', 'Montréal', 'Canada', 'jean.dupont@email.com', 'mdp123'),
	('Smith', 'Alice', 'Toronto', 'Canada', 'alice.smith@email.com', 'alice2025'),
	('Nguyen', 'Minh', 'Paris', 'France', 'minh.nguyen@email.com', 'nguyen!pwd');

INSERT INTO vehicule(marque,modele,plaqueImmatriculation,couleur)
	VALUES
	('Toyota', 'Corolla', 'ABC123', 'Rouge'),
	('Honda', 'Civic', 'XYZ789', 'Bleu'),
	('Tesla', 'Model 3', 'EV2025', 'Blanc');

INSERT INTO capteur(mouvement,date)
	VALUES
	(1.00, '2025-09-01'),
	(0.50, '2025-09-15'),
	(0.00, '2025-09-19');

INSERT INTO stationnement(nombrePlaceMax,dureeMaxStationnement,entreSortieStationnement,tarif,estPlein)
	VALUES
	(20, '24:00:00', 0, 5.00, 0);

INSERT INTO barriere(dureeAttente,noBarriereOuverture,tempsOuverture,numeroCapteur)
	VALUES
	(5, 1, '08:00:00', 1),
	(3, 2, '09:30:00', 2),
	(10, 1, '10:15:00', 3);
INSERT INTO stationnementEntreeSortie(dateEntree,dateSortie,paiementSortie,paiementRecu,numVehicule,numVehicule,numBarriere,numUtilisateur,reservation)
	values
	('2025-09-10', '2025-09-10', 5.00, 1, 1, 1, 1, 0),
	('2025-09-15', '2025-09-15', 7.50, 1, 2, 2, 2, 1),
	('2025-09-18', '2025-09-18', 10.00, 0, 3, 3, 3, 0);