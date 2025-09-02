# 🏥 GAIA Reservation

> **API complète pour réserver, facturer et gérer des cabinets médicaux**

[![.NET 8](https://img.shields.io/badge/.NET-8-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-13+-blue.svg)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-Proprietary-red.svg)](LICENSE)

---

## ✨ Fonctionnalités

### 🔐 Authentification
- **Login/Register** : Système d'inscription et de connexion
- **JWT Token** : Gestion des tokens d'accès et de rafraîchissement
- **Sécurité renforcée** : Authentification à double facteur (OTP)

### 👥 Administration
- **Gestion des créneaux** : Configuration des disponibilités des cabinets
- **Suivi complet** : Réservations, factures et clients
- **Gestion utilisateurs** : Administration des comptes et permissions

### 📅 Réservation
- **Interface médecins** : Réservation simple selon les créneaux disponibles
- **Gestion des conflits** : Évitement des doubles réservations

### 🧹 Entretien
- **Interface personnel** : Vue dédiée pour connaître les cabinets à nettoyer
- **Planning optimisé** : Organisation efficace des tâches de maintenance

---

## 🛠️ Installation

### Prérequis

Assurez-vous d'avoir installé :
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 13+](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)

### 🐳 Base de données PostgreSQL

Un fichier `docker-compose.yml` est disponible dans le dossier `/help` pour démarrer rapidement PostgreSQL :

```bash
cd help
docker-compose up -d
```

### 📥 Cloner et configurer le projet

```bash
# Cloner le repository
git clone https://github.com/BastienGRNT/gaia-cabinet_backend.git
cd gaia-cabinet_backend

# Restaurer les dépendances
cd GaiaSolution.Application
dotnet restore
dotnet build
```

---

## ⚙️ Configuration

### Variables d'environnement

L'application utilise un fichier `.env` pour la configuration. Créez ce fichier à la racine du projet avec les variables suivantes :

> ⚠️ **Note importante** : Les valeurs ci-dessous sont des exemples à des fins pédagogiques. En production, utilisez des clés sécurisées et des mots de passe complexes.

```env
# 🗄️ Base de données
DB_CONNECTION_STRING=Host=localhost;Port=5432;Database=gaiacabinet_dev;Username=dev_user;Password=dev_password;Include Error Detail=true

# 🔑 Authentification JWT
AUTHJWT__ISSUER=http://localhost:5111
AUTHJWT__AUDIENCE=http://localhost:3000
AUTHJWT__SIGNINGKEY=UNE_CLE_TRES_LONGUE_ET_ALEATOIRE_MIN_32_CHARS
AUTHJWT__ACCESSTOKENMINUTES=15
AUTHJWT__REFRESHTOKENDAYS=30

# 🔐 OTP (One-Time Password)
OTPPEPPER__OTPPEPPER=UNE_CLE_TRES_LONGUE_ET_ALEATOIRE_MIN_32_CHARS
```

### 📝 Fichier d'exemple

Un fichier `.env.example` sera fourni à la racine du projet pour faciliter la configuration. Copiez-le et renommez-le en `.env`, puis adaptez les valeurs selon votre environnement.

---

## 🚀 Lancement

Une fois la configuration terminée, lancez l'application :

```bash
cd GaiaSolution.Application
dotnet run
```

L'API sera accessible sur `http://localhost:5111` (ou le port configuré).

---

## 📚 Documentation

### Structure du projet
```
GaiaSolution/
├── GaiaSolution.Api/            # Point d'entrée de l'API (Controllers, Startup)
├── GaiaCabinet.Application/     # Logique métier (Services, DTOs, Handlers)
├── GaiaCabinet.Domain/          # Entités métier et règles de domaine
├── GaiaCabinet.Infrastructure/  # Accès aux données (DbContext, Repositories)
├── help/                        # Docker Compose pour PostgreSQL, etc...
├── .env.example                 # Exemple de configuration des variables d'environement
├── LICENSE                      # License de l'application
└── README.md                    # Ce fichier
```

### API Endpoints

La documentation complète des endpoints sera disponible via Swagger une fois l'application lancée :
- **Swagger UI** : `http://localhost:5111/swagger`

---

## 🤝 Contribution

Ce projet étant sous licence propriétaire, les contributions externes ne sont pas acceptées actuellement. 

Pour toute suggestion ou signalement de bug, merci d'utiliser les [GitHub Issues](https://github.com/BastienGRNT/gaia-cabinet_backend/issues).

---

## 📄 License

Ce projet est sous licence propriétaire.  
**Copyright (c) 2025 GRENOT Bastien. Tous droits réservés.**

Toute copie, modification, distribution ou utilisation non autorisée de ce logiciel, par tout moyen que ce soit, est strictement interdite.

Voir le fichier [LICENSE](LICENSE) pour les détails complets.

---

## 📞 Support

Pour toute question ou problème :
- 🐛 **Issues** : [GitHub Issues](https://github.com/BastienGRNT/gaia-cabinet_backend/issues)
- 📧 **Contact** : bastien.grenot@gmail.com

---
