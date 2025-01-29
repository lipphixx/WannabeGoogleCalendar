<template>
  <div class="login">
    <h2>Login</h2>
    <form @submit.prevent="login">
      <div>
        <label for="username">Username</label>
        <input
            type="text"
            id="username"
            v-model="username"
            placeholder="Enter username"
            required
        />
      </div>
      <div>
        <label for="password">Password</label>
        <input
            type="password"
            id="password"
            v-model="password"
            placeholder="Enter password"
            required
        />
      </div>
      <button type="submit">Log In</button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
  </div>
</template>

<script>
export default {
  data() {
    return {
      username: "",
      password: "",
      errorMessage: null,
      // Přednastavené uživatelské údaje (můžete to nahradit voláním API)
      validUsername: "admin",
      validPassword: "admin123",
    };
  },
  methods: {
    login() {
      // Ověření uživatele (v tomto případě jednoduché porovnání)
      if (this.username === this.validUsername && this.password === this.validPassword) {
        // Přihlášení bylo úspěšné
        alert("Login successful!");
        // Můžete přesměrovat uživatele po úspěšném přihlášení
        this.$router.push("/dashboard");
      } else {
        // Chyba při přihlášení
        this.errorMessage = "Invalid username or password.";
      }
    },
  },
};
</script>

<style scoped>
.login {
  width: 300px;
  margin: auto;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 5px;
}

form div {
  margin-bottom: 10px;
}

input {
  width: 100%;
  padding: 8px;
  margin: 5px 0;
  border-radius: 4px;
  border: 1px solid #ddd;
}

button {
  width: 100%;
  padding: 10px;
  background-color: #4caf50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #45a049;
}

.error {
  color: red;
  font-size: 14px;
  margin-top: 10px;
}
</style>
