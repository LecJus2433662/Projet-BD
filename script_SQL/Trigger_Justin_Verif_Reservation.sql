USE Prog_A25_Bd_Projet;
GO

CREATE TRIGGER trg_Verif_Date_Reservation_Utilisateur
ON stationnementEntreeSortie
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @reservation BIT;
    DECLARE @numUtilisateur INT;
    DECLARE @dateEntree DATE;
    DECLARE @dateSortie DATE;

    SELECT  
        @reservation   = reservation,
        @numUtilisateur = numUtilisateur,
        @dateEntree    = dateEntree,
        @dateSortie    = dateSortie
    FROM stationnementEntreeSortie;

	
	IF @dateEntree < GETDATE() AND @reservation = 1
	BEGIN 
		PRINT('ERREUR : la date de entrée est déjà passée pour une réservation')
		ROLLBACK TRANSACTION
	END;

    IF @reservation = 1 AND @dateSortie < GETDATE()
    BEGIN
        PRINT('Erreur : la date de sortie est déjà passée pour une réservation.');
        ROLLBACK TRANSACTION;
        RETURN;
    END

	IF @dateEntree < @dateSortie
	BEGIN
        PRINT('Erreur : la date de sortie doit être après ou dans à la même date de la date d''entrée');
		ROLLBACK TRANSACTION
	END;
END;
GO
