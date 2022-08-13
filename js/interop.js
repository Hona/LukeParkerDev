window.highlightBlogPostCode = function() {
    document.querySelectorAll('.blog-markdown code').forEach((el) => {
        hljs.highlightElement(el);
    });
}