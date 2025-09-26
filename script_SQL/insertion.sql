USE Prog_A25_BD_Projet;

INSERT INTO vehicule(marque,modele,plaqueImmatriculation,couleur)
	VALUES
	('Toyota', 'Corolla', 'ABC123', 'Rouge'),
	('Honda', 'Civic', 'XYZ789', 'Bleu'),
	('Tesla', 'Model 3', 'EV2025', 'Blanc');

INSERT INTO capteur(mouvement,dates)
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
INSERT INTO stationnementEntreeSortie(dateEntree,dateSortie,paiementSortie,paiementRecu,reservation)
	values
	('2025-09-10', '2025-09-10',1, 5.00, 0),
	('2025-09-15', '2025-09-15',1, 7.50,  1),
	('2025-09-18', '2025-09-18',1, 10.00,  0);