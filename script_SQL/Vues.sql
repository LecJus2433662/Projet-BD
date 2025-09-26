CREATE VIEW vue_Stationnements_Vehicules AS
SELECT 
    ses.entreSortieStationnement,
    ses.dateEntree,
    ses.dateSortie,
    ses.paiementSortie,
    ses.paiementRecu,
    ses.reservation,
    v.numVehicule,
    v.marque,
    v.modele,
    v.plaqueImmatriculation,
    v.couleur,
    b.numBarriere,
    b.tempsOuverture
FROM stationnementEntreeSortie ses
JOIN vehicule v ON ses.numVehicule = v.numVehicule
JOIN barriere b ON ses.numBarriere = b.numBarriere;
GO
CREATE VIEW vue_Barrieres_Capteurs AS
SELECT 
    b.numBarriere,
    b.dureeAttente,
    b.tempsOuverture,
    c.numCapteur,
    c.mouvement,
    c.dates AS dateCapteur
FROM barriere b
JOIN capteur c ON b.numeroCapteur = c.numCapteur;
