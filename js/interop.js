window.highlightBlogPostCode = function() {
    document.querySelectorAll('.blog-markdown code').forEach((el) => {
        hljs.highlightElement(el);
    });
}

window.scrollDownFullPage = function() {
    const appBarHeight = document.getElementsByClassName("mud-appbar")[0].clientHeight;
    
    window.scrollBy({
        top: window.innerHeight - appBarHeight,
        left: 0,
        behavior : "smooth"
    });
}

window.loadDisqus = function(relativeUrl, slug) {
    window.disqus_config = function () {
        this.page.url = relativeUrl;  // Replace PAGE_URL with your page's canonical URL variable
        this.page.identifier = slug; // Replace PAGE_IDENTIFIER with your page's unique identifier variable
    };

    const d = document,
        s = d.createElement('script');
    s.src = 'https://lukeparker-dev.disqus.com/embed.js';
    s.setAttribute('data-timestamp', + new Date());
    (d.head || d.body).appendChild(s);
}
    