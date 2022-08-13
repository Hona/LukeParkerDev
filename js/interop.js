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