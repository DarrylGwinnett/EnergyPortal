<template>
  <aside class="sidebar">
    <div class="sidebar-header">
      <h2>Sidebar</h2>
    </div>
    <div class="sidebar-content">
      <ul>
        <li v-for="page in pages" :key="page.id">
          <router-link :to="page.route">{{ page.name }}</router-link>
        </li>
      </ul>
    </div>
  </aside>
</template>

<script>
export default {
  name: 'SideBar',
  data() {
    return {
      pages: [
        { id: 1, name: 'Home', route: '/' },
        { id: 2, name: 'About', route: '/about' },
        { id: 3, name: 'Contact', route: '/contact' }
      ]
    };
  }
};
</script>

<style scoped>
.sidebar {
  width: 20%; 
  height: 100vh;
  background-color: #f4f4f4;
  overflow-y: auto; 
}

.sidebar-header {
  background-color: #333;
  color: #fff;
  padding: 1rem;
  text-align: center;
}

.sidebar-content {
  padding: 1rem;
}

ul {
  list-style: none;
  padding: 0;
}

li {
  margin-bottom: 0.5rem;
}

router-link {
  text-decoration: none;
  color: #333;
}

router-link.active {
  font-weight: bold;
}
</style>