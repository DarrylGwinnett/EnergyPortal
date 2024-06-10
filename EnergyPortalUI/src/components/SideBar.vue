<template>
  <aside :class="['sidebar', { collapsed: isCollapsed }]">
    <div class="sidebar-header">
      <h2>Sidebar</h2>
      <button class="collapse-button" @click="toggleCollapse">
        <i :class="[isCollapsed ? 'fas fa-chevron-right' : 'fas fa-chevron-left']"></i>
      </button>
    </div>
    <div class="sidebar-content" v-show="!isCollapsed">
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
      isCollapsed: false,
      pages: [
        { id: 1, name: 'Home', route: '/' },
        // Add more pages as needed
      ]
    };
  },
  methods: {
    toggleCollapse() {
      this.isCollapsed = !this.isCollapsed;
    }
  }
};
</script>

<style scoped>
.sidebar {
  width: 20%;
  height: 100vh;
  background-color: #f4f4f4;
  overflow-y: auto;
  transition: width 0.3s ease;
}

.sidebar.collapsed {
  width: 3%;
}

.sidebar-header {
  background-color: #333;
  color: #fff;
  padding: 1rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
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

.collapse-button {
  background: none;
  border: none;
  color: #fff;
  cursor: pointer;
  font-size: 1rem;
}

.collapse-button .fas {
  transition: transform 0.3s ease;
}
</style>
