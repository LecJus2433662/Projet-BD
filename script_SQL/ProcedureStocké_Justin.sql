USE Prog_A25_BD_Projet;
GO

CREATE OR ALTER PROCEDURE sp_ListerUtilisateurEtReservations
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @noUtilisateur INT,
        @nom VARCHAR(30),
        @prenom VARCHAR(30),
        @nbReservations INT;

    -- Déclaration du curseur
    DECLARE utilisateur_cursor CURSOR FOR
        SELECT noUtilisateur, nom, prenom
        FROM utilisateur;

    -- Ouvrir le curseur
    OPEN utilisateur_cursor;

    -- Lire la première ligne
    FETCH NEXT FROM utilisateur_cursor INTO @noUtilisateur, @nom, @prenom;

    -- Boucle tant qu'il reste des résultats
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Compter les entrées/sorties du stationnement pour cet utilisateur
        SELECT @nbReservations = COUNT(*)
        FROM stationnementEntreeSortie
        WHERE numUtilisateur = @noUtilisateur;

        -- Affichage
        PRINT 'Utilisateur : ' 
              + CAST(@noUtilisateur AS VARCHAR(10)) 
              + ' - ' + @prenom + ' ' + @nom 
              + ' | Réservations : ' + CAST(@nbReservations AS VARCHAR(10));

        -- Fetch suivant
        FETCH NEXT FROM utilisateur_cursor INTO @noUtilisateur, @nom, @prenom;
    END;

    -- Nettoyage
    CLOSE utilisateur_cursor;
    DEALLOCATE utilisateur_cursor;
END;
GO
