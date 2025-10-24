
USE Prog_A25_BD_Projet_Prog;

/* ADD Foreign Keys*/
ALTER TABLE stationnement 
ADD CONSTRAINT  fk_entreSortieStationnement
FOREIGN KEY (entreSortieStationnement) REFERENCES stationnementEntreeSortie(entreSortieStationnement);

ALTER TABLE barriere 
ADD CONSTRAINT  fk_numCapteur
FOREIGN KEY (numeroCapteur) REFERENCES capteur(numCapteur);

ALTER TABLE stationnementEntreeSortie  
ADD CONSTRAINT  fk_numVehicule
FOREIGN KEY (numVehicule) REFERENCES vehicule(numVehicule);

ALTER TABLE stationnementEntreeSortie 
ADD CONSTRAINT  fk_numBarriere
FOREIGN KEY (numBarriere) REFERENCES barriere(numBarriere);

/*ADD UNIQUE*/

ALTER TABLE vehicule
ADD CONSTRAINT unique_plaque
UNIQUE (plaqueImmatriculation);

ALTER TABLE utilisateur
ADD CONSTRAINT unique_email
UNIQUE (email);

/*ADD CHECK*/

ALTER TABLE stationnementEntreeSortie
ADD CONSTRAINT chk_dates CHECK (dateSortie IS NULL OR dateSortie >= dateEntree);

/*ADD Default*/
ALTER TABLE stationnementEntreeSortie
ADD CONSTRAINT df_reservation DEFAULT 0 FOR reservation;

ALTER TABLE stationnementEntreeSortie
ADD CONSTRAINT df_dateEntree DEFAULT GETDATE() FOR dateEntree;



